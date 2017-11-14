import React, { Component } from 'react'
//import Hello from './components/hello'
import { } from 'bootstrap-css'
import { PageHeader } from 'react-bootstrap'
import InputForm from './components/InputForm'
import CodeList from './components/CodeList'
import DeleteButton from './components/DeleteButton'


const baseurl = "http://192.168.88.46:8081/api/pastebin"
class App extends Component {

  constructor(){
    super();
    this.state = {
      codesnippets : []
    };
  }

  Reloadapi(){
    fetch(baseurl)
    .then(response=>response.json())
    .then(json=>{
      this.setState({codesnippets:json})
    })
    .catch(error=>console.error(error))
  }
  componentDidMount(){
    console.debug('App mounted, retrieving snippets from the server...');
    this.Reloadapi();
    console.debug('Code snippets retrieved');
  }
  
  render() {
    return (
      <div>
        <PageHeader>Pastebin <small>v0.6</small></PageHeader>
        <InputForm onPaste={(pastedcode)=>{
          fetch(baseurl,{
            method: 'POST',
            headers: {
              'Accept': 'application/json',
              'Content-Type': 'application/json',
            },
            body: JSON.stringify({"pastedcode":pastedcode})
          })
          .then(response=>response.json())
          .then(json=>{
            this.setState({codesnippets:json})
          })
        }} />
        <CodeList codesnippets={this.state.codesnippets} />
        <DeleteButton onDelete={()=>{
          console.debug('delete button clicked')
          if (window.confirm('Are you sure?')){
            console.debug('Delete all snippets confirmed, calling API...')
            fetch(baseurl,{
              method: 'DELETE',
              headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
              }
            })
            .then(response=>response.json())
            .then(json=>{
              this.setState({codesnippets:json})
            })
            console.debug('All snippets deleted.')
          }
        }}/>
      </div>
    );
  }
}

export default App;
