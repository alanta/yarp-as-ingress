// Import the LitElement base class and html helper function
import { LitElement, html } from 'lit-element';

// Extend the LitElement base class
class MyElement extends LitElement {
  override createRenderRoot() {
    return this;
  }
  override render(){
    return html`

    <nav class="navbar navbar-expand-lg navbar-dark bg-dark fixed-top">
      <div class="container">
        <a class="navbar-brand" href="#">Start Bootstrap</a>
        <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarResponsive" aria-controls="navbarResponsive" aria-expanded="false" aria-label="Toggle navigation">
          <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarResponsive">
          <ul class="navbar-nav ml-auto">
            <li class="nav-item active">
              <a class="nav-link" href="#">Home
                <span class="sr-only">(current)</span>
              </a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="#">About</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="#">Services</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="#">Contact</a>
            </li>
          </ul>
        </div>
      </div>
    </nav> 
  <nav aria-label="breadcrumb">
    <ol class="breadcrumb">
      <li class="breadcrumb-item"><a href="#">Home</a></li>
      <li class="breadcrumb-item"><a href="#">Library</a></li>
      <li class="breadcrumb-item active" aria-current="page">Data</li>
    </ol>
  </nav>
    <!-- Page Content -->
    <div class="container">

      <!-- Jumbotron Header -->
      <header class="jumbotron my-4">
        <h1 class="display-3">A Warm Welcome!</h1>
        <p class="lead">Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ipsa, ipsam, eligendi, in quo sunt possimus non incidunt odit vero aliquid similique quaerat nam nobis illo aspernatur vitae fugiat numquam repellat.</p>
        <a href="#" class="btn btn-primary btn-lg">Call to action!</a>
      </header>
     <div class="row text-center">
        <div class="col-lg-12 col-md-12 mb-12">
          <h3>Some cards</h3>
         </div>
      </div>
      <!-- Page Features -->
      <div class="row text-center">

        <div class="col-lg-3 col-md-6 mb-4">
          <div class="card">
            <img class="card-img-top" src="https://raw.githubusercontent.com/guzmanpaniagua/basic-example-lit-element-with-bootstrap/master/500-325.jpg" alt="">
            <div class="card-body">
              <h4 class="card-title">Card title</h4>
              <p class="card-text">Lorem ipsum dolor sit amet, consectetur adipisicing elit. Sapiente esse necessitatibus neque.</p>
            </div>
            <div class="card-footer">
              <a href="#" class="btn btn-primary">Find Out More!</a>
            </div>
          </div>
        </div>

        <div class="col-lg-3 col-md-6 mb-4">
          <div class="card">
            <img class="card-img-top" src="https://raw.githubusercontent.com/guzmanpaniagua/basic-example-lit-element-with-bootstrap/master/500-325.jpg" alt="">
            <div class="card-body">
              <h4 class="card-title">Card title</h4>
              <p class="card-text">Lorem ipsum dolor sit amet, consectetur adipisicing elit. Explicabo magni sapiente, tempore debitis beatae culpa natus architecto.</p>
            </div>
            <div class="card-footer">
              <a href="#" class="btn btn-primary">Find Out More!</a>
            </div>
          </div>
        </div>

        <div class="col-lg-3 col-md-6 mb-4">
          <div class="card">
            <img class="card-img-top" src="https://raw.githubusercontent.com/guzmanpaniagua/basic-example-lit-element-with-bootstrap/master/500-325.jpg" alt="">
            <div class="card-body">
              <h4 class="card-title">Card title</h4>
              <p class="card-text">Lorem ipsum dolor sit amet, consectetur adipisicing elit. Sapiente esse necessitatibus neque.</p>
            </div>
            <div class="card-footer">
              <a href="#" class="btn btn-primary">Find Out More!</a>
            </div>
          </div>
        </div>

        <div class="col-lg-3 col-md-6 mb-4">
          <div class="card">
            <img class="card-img-top" src="https://raw.githubusercontent.com/guzmanpaniagua/basic-example-lit-element-with-bootstrap/master/500-325.jpg" alt="">
            <div class="card-body">
              <h4 class="card-title">Card title</h4>
              <p class="card-text">Lorem ipsum dolor sit amet, consectetur adipisicing elit. Explicabo magni sapiente, tempore debitis beatae culpa natus architecto.</p>
            </div>
            <div class="card-footer">
              <a href="#" class="btn btn-primary">Find Out More!</a>
            </div>
          </div>
        </div>

      </div>
      <!-- /.row -->


      <div class="row text-center">

        <div class="col-lg-12 col-md-12 mb-12">
        <h3>Some data</h3>
        <table class="table table-striped">
          <thead class="thead-dark">
            <tr>
              <th scope="col">#</th>
              <th scope="col">First</th>
              <th scope="col">Last</th>
              <th scope="col">Handle</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <th scope="row">1</th>
              <td>Mark</td>
              <td>Otto</td>
              <td>@mdo</td>
            </tr>
            <tr>
              <th scope="row">2</th>
              <td>Jacob</td>
              <td>Thornton</td>
              <td>@fat</td>
            </tr>
            <tr>
              <th scope="row">3</th>
              <td>Larry</td>
              <td>the Bird</td>
              <td>@twitter</td>
            </tr>
          </tbody>
        </table>
        </div>
      </div>
      <!-- /.row -->

     <div class="row text-center">
        <div class="col-lg-12 col-md-12 mb-12">
          <h3>Some panels</h3>
         </div>
      </div>

     <div class="row text-center">

        <div class="col-lg-4 col-md-6 mb-4">
          <div class="card" >
            <div class="card-header bg-primary text-white">
              Featured
            </div>
            <ul class="list-group list-group-flush">
              <li class="list-group-item">Cras justo odio</li>
              <li class="list-group-item">Dapibus ac facilisis in</li>
              <li class="list-group-item">Vestibulum at eros</li>
            </ul>
          </div>
        </div>

        <div class="col-lg-4 col-md-6 mb-4">
          <div class="card" >
            <div class="card-header bg-primary text-white">
              Featured
            </div>
            <ul class="list-group list-group-flush">
              <li class="list-group-item">Cras justo odio</li>
              <li class="list-group-item">Dapibus ac facilisis in</li>
              <li class="list-group-item">Vestibulum at eros</li>
            </ul>
          </div>
        </div>

        <div class="col-lg-4 col-md-6 mb-4">
          <div class="card" >
            <div class="card-header bg-primary text-white">
              Featured
            </div>
            <ul class="list-group list-group-flush">
              <li class="list-group-item">Cras justo odio</li>
              <li class="list-group-item">Dapibus ac facilisis in</li>
              <li class="list-group-item">Vestibulum at eros</li>
            </ul>
          </div>
        </div>
      </div>
   <!-- /.row -->

     <div class="row ">
        <div class="col-lg-12 col-md-12 mb-12">
          <h3 class="text-center">Some form</h3>
          
          <form>
            <div class="form-group">
              <label for="exampleInputEmail1">Email address</label>
              <input type="email" class="form-control" id="exampleInputEmail1" aria-describedby="emailHelp" placeholder="Enter email">
              <small id="emailHelp" class="form-text text-muted">We'll never share your email with anyone else.</small>
            </div>
            <div class="form-group">
              <label for="exampleInputPassword1">Password</label>
              <input type="password" class="form-control" id="exampleInputPassword1" placeholder="Password">
            </div>
            <div class="form-check">
              <input type="checkbox" class="form-check-input" id="exampleCheck1">
              <label class="form-check-label" for="exampleCheck1">Check me out</label>
            </div>
            <button type="submit" class="btn btn-primary">Submit</button>
          </form>
          
         </div>
      </div>

      <div class="row text-center">
        <div class="col-lg-12 col-md-12 mb-12">
         <h3 class="text-center">Some progress bars</h3>
         </div>
      </div>
          
      <div class="row ">
        <div class="col-lg-12 col-md-12 mb-12">
      
          <div class="card" >

            <ul class="list-group list-group-flush">
              <li class="list-group-item">
                <div class="progress">
                  <div class="progress-bar text-left" role="progressbar" style="width: 95%; padding-left:20px;" aria-valuenow="95" aria-valuemin="0" aria-valuemax="100"> Javascript 95%</div>
                </div>
              </li>
              <li class="list-group-item">
                <div class="progress">
                  <div class="progress-bar text-left" role="progressbar" style="width: 85%; padding-left:20px;" aria-valuenow="95" aria-valuemin="0" aria-valuemax="100"> Lit Element 85%</div>
                </div>
              </li>
              <li class="list-group-item">
                <div class="progress">
                  <div class="progress-bar text-left" role="progressbar" style="width: 90%; padding-left:20px;" aria-valuenow="90" aria-valuemin="0" aria-valuemax="100"> Css 90%</div>
                </div>
              </li>
              <li class="list-group-item">
                <div class="progress">
                  <div class="progress-bar text-left" role="progressbar" style="width: 80%; padding-left:20px;" aria-valuenow="80" aria-valuemin="0" aria-valuemax="100"> PWA 80%</div>
                </div>
              </li>
            </ul>
          </div>
 
       
       
        
        </div>
      </div>
 <div class="row ">
        <div class="col-lg-12 col-md-12 mb-12">
&nbsp;
    </div>
      </div>



          </div>
    <!-- /.container -->
    <!-- Footer -->
    <footer class="py-5 bg-dark">
      <div class="container">
        <p class="m-0 text-center text-white">Copyright &copy; Your Website 2018</p>
      </div>
      <!-- /.container -->
    </footer>
    `;
  }
}
// Register the new element with the browser.
customElements.define('my-element', MyElement);
