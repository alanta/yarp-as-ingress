// This is sample code that calls the HubspotChatBot API
// Use this to run manual local tests against the API

const request = require('request');
const crypto = require('crypto');

exports.main = (event, callback) => {
  const timestamp = event.timestamp;
  const options = {
    //url: 'https://testconnect.milgro.nl/hubspotchatbot/checkOrderNumber',
    url: 'http://localhost:5204/api/hello/world',
    rejectUnauthorized: false, // for local testing only!
    method: 'GET',
    body:  event.userMessage ? JSON.stringify(event.payload) : null,
    headers: {
      'Content-Type': 'application/json',
      'User-Agent': 'Test',
      'X-Request-Signature': '',
      'X-Request-Timestamp': timestamp
    }
  };

  // https://ckeditor.com/docs/cs/latest/examples/security/request-signature-nodejs.html
  function generateSignature( apiSecret, method, uri, timestamp, body ) {
    console.log('hash over = '+`${ method.toUpperCase() }${ uri }${ timestamp }`)
     
     const hmac = crypto.createHmac( 'SHA256', apiSecret );
 
     hmac.update( `${ method.toUpperCase() }${ uri }${ timestamp }` );
 
     if ( body ) {
         hmac.update( Buffer.from( body ) );
     }
 
     return hmac.digest('base64');
   }

  options.headers['X-Request-Signature']= generateSignature( 'SECRET', options.method, options.url, timestamp, options.body );

  console.log('signature: '+options.headers['X-Request-Signature'])

  request(options, function (error, response, body) {
    if(error)
      {
        console.log("Error: "+error)
        return;
      }
    if(response.statusCode > 299)
    {
      console.log('Request failed : '+response.statusCode)
      return;
    }

    if(!response.headers['content-type'] || !response.headers['content-type'].startsWith('application/json')){
      console.log('Unexpected response type : '+response.headers['content-type'])
      return;
    }

    const responseJson = JSON.parse(body);

    callback(responseJson);

  });
}

exports.main(
  {
    payload: null,
    timestamp: Date.now()
  },
  function(x){ console.log(x) })
/* 	
  SAMPLE EVENT OBJECT
  What we send you

  {
    // userMessage is the message your visitor has sent to your bot.
    userMessage: {
      message: '200-300 Employees',
      quickReply: {
        quickReplies:[
          {
              value:'100-500',
              label:'100-500'
          }
        ]
      }
    },
    session: {
      // This is the visitorId.
      // If you collect an email, or if the visitor 
      // is already a contact, this will map to a contact id in HubSpot CRM.
      vid: 12345,
      // This contains all the properties your bot has collected at the moment.
      // For example, if you had collected a HubSpot contact property called FavoriteColor,
      // it would be listed here.
      properties: {
        CONTACT: {
          firstname: {
            value: 'John',
            syncedAt: 1534362540592
          }
        },
        COMPANY: {
          name: {
            value: 'HubSpot',
            syncedAt: 1534362540592
          }
        }
      }
    }
  }

  EXAMPLE RESULT OBJECTS
  What your code can return

  Example 1 : Send a message and goto another bot module.

  {
    // The message your bot will return.
    botMessage: 'Thanks for checking out our website {{ contact.firstname }}',
    // The next module your bot will go to. If nothing is provided,
    // we will select the next module in the bot path for you.
    nextModuleNickname: 'SuggestAwesomeProduct',
    // Whether or not this code snippet should be executed again with the next user input.
    responseExpected: false
  }

  Example 2 : Send a message from your code snippet THEN send response to question back into your snippet.

  {
    botMessage: 'Whats your favorite color?',
    responseExpected: true
  }
*/

