// Import the LitElement base class and html helper function
import { LitElement, html } from 'lit';
import './api-action'

// Extend the LitElement base class
class Layout extends LitElement {
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
          </ul>
        </div>
      </div>
    </nav> 
    <!-- Page Content -->
    <div class="container">

      <!-- Jumbotron Header -->
      <header class="jumbotron my-4">
        <h1 class="display-3">Yarp as Ingress</h1>
        <p class="lead">This is a tiny SPA served by a minimal .NET web app.</p>
      </header>
  
     <div class="row">
        <api-action></api-action>
      </div>

    </div>
    <!-- /.container -->
    <!-- Footer -->
    <footer class="py-5 bg-dark">
      <div class="container">
        <p class="m-0 text-center text-white">Copyright &copy; Your Website 2023</p>
      </div>
      <!-- /.container -->
    </footer>
    `;
  }
}
// Register the new element with the browser.
customElements.define('app-layout', Layout);
