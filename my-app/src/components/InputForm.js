import React, { Component } from "react";
import { Form,FormGroup,FormControl,Col,ControlLabel,Button } from 'react-bootstrap';

class InputForm extends Component {

  constructor(props){
    super(props);
    this.state = {
      codebody:''
    }
    this.onPaste = props.onPaste;
  }

  render(){
      return (
          <Form horizontal>
          <FormGroup controlId="formCode">
            <Col sm={10}>
              <FormControl type="text" placeholder="..." 
              componentClass="textarea"
              onChange={event=>this.setState({codebody:event.target.value})}
              rows="15" />
            </Col>
          </FormGroup>
              
          <FormGroup>
            <Col smOffset={2} sm={10}>
              <Button bsStyle="primary" onClick={()=>{
                console.debug('save button clicked');
                this.onPaste(this.state.codebody);
              }} block>
                Save My Code Snippet
              </Button>
            </Col>
          </FormGroup>
        </Form>
      )
  }
}

export default InputForm;