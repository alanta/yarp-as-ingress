// This is sample code that calls the API with HMAC authentication
// Use this to run manual local tests against the API

const request = require('request');
const crypto = require('crypto');

const AuthType = 'HMAC-SHA256'
const AuthKeyId = 'Demo'
const AuthKeySecret = 'SECRET'

requestWithHMAC = (payload, timestamp, callback) => {
  const options = {
    url: 'http://localhost:5204/api/hello/world',
    rejectUnauthorized: false, // for local testing only!
    method: 'GET',
    body:  payload ? JSON.stringify(payload) : null,
    headers: {
      'Content-Type': 'application/json',
      'User-Agent': 'Test',
      'X-Request-Timestamp': timestamp
    }
  };

  function generateSignature( apiSecret, method, uri, timestamp, body ) {
    console.log('hash over = '+`${ method.toUpperCase() }${ uri }${ timestamp }`)
     
     const hmac = crypto.createHmac( 'SHA256', apiSecret );
 
     hmac.update( `${ method.toUpperCase() }${ uri }${ timestamp }` );
 
     if ( body ) {
         hmac.update( Buffer.from( body ) );
     }
 
     return hmac.digest('base64');
   }

  var signature = generateSignature( AuthKeySecret, options.method, options.url, timestamp, options.body )
  console.log('signature: '+signature)

  options.headers['Authorization']= `${AuthType} id=${AuthKeyId};signature=${signature}`;

  request(options, function (error, response, body) {
    if(error)
      {
        console.log("Error: "+error)
        return;
      }
    if(response.statusCode > 299)
    {
      console.log('Request failed : '+response.statusCode)
      if( response.body )
        console.log(response.body)
        
      console.log()
      return;
    }

    if(!response.headers['content-type'] || !response.headers['content-type'].startsWith('application/json')){
      callback(body);
      return;
    }
    else{
      const responseJson = JSON.parse(body);
      callback(responseJson);
    }
  });
}

requestWithHMAC(payload=null, timestamp=Date.now(), callback=function(x){ console.log(x) })
