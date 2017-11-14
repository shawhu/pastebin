import React, { Component } from "react";
import { Button,Col } from 'react-bootstrap';

class DeleteButton extends Component {
    render(){
        return(
            <Col smOffset={2} sm={10}>
            <br /><br /><br /><br /><br /><br />
            <Button bsStyle="danger" block onClick={()=>{
                this.props.onDelete();
            }}>Remove All Snippets</Button>
            </Col>
        )
    }
}

export default DeleteButton;