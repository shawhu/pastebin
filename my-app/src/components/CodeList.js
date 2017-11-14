import React, { Component } from "react";
import CodeListItem from './CodeListItem'
import { Col } from 'react-bootstrap';

class CodeList extends Component {
    render() {
        const Items = this.props.codesnippets.map((snippet) => {
            return <CodeListItem key={snippet.codeid} snippet={snippet} />
        });
        return (
            <div>
            <Col sm={10}>Click the code to copy to the clipboard.</Col>
            <ul>{Items}</ul>
            </div>
        )
    }
}

export default CodeList;