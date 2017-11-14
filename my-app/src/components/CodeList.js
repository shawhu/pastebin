import React, { Component } from "react";
import CodeListItem from './CodeListItem'
import { Form,FormGroup,FormControl,Col,ControlLabel,Button } from 'react-bootstrap';

class CodeList extends Component {
    render() {
        const Items = this.props.codesnippets.map((snippet) => {
            return <CodeListItem key={snippet.codeid} snippet={snippet} />
        });
        return (
            <div>
            <Col sm={10}>Clicking on code to copy it to the clipboard.</Col>
            <ul>{Items}</ul>
            </div>
        )
    }
}

export default CodeList;