import { LitElement, html } from 'lit'
import {when} from 'lit/directives/when'

class ApiAction extends LitElement {
    
    dataLoaded: boolean = false;
    hasError: boolean = false;
    reply: string = '';
    error: string = '';
    override createRenderRoot() {
        return this;
    }
    get input() {
        return this.renderRoot?.querySelector('#inputName') ?? null;    
    }

    async _callApi(event: Event) {
        event.preventDefault();
        try {
            this.dataLoaded = false;
            this.hasError = false;
            const response = await fetch(`/api/hello/${this.input.value}`);
            
            if (response.status > 299) {
                this.hasError = true;
                this.error = `Request failed with status ${response.status}`
                return;
            }
            var text = await response.text();
            if (text.length > 0) {
                this.dataLoaded = true;
                this.reply = text;
            }
            else {
                this.hasError = true;
                this.error = "API returned no data";
            }
        }
        catch (error) {
            console.log(error);
            this.hasError = true;
        }
        finally {
            this.requestUpdate();
        }
    }

    override render(){
        return html`
        <div class="col-lg-12 col-md-12 mb-5 ">
        <h3>Call the API</h3>
        
        <form @submit=${this._callApi}>
          <div class="form-group">
            <label for="inputName">Name</label>
            <input type="text" class="form-control" id="inputName" aria-describedby="nameHelp" placeholder="Jane">
            <small id="nameHelp" class="form-text text-muted">Please tell us your name.</small>
          </div>
          <button type="submit" class="btn btn-primary">Submit</button>
        </form>
        
       
        ${when(this.hasError, () => html`<div class="alert alert-danger">${this.error}</div>`)}
        ${when(this.dataLoaded, () => html`<div class="alert alert-success">${this.reply}</div>`)}
        </div>
    `
    }
}

customElements.define('api-action', ApiAction)