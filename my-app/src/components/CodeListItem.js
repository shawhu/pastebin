import React, { Component } from "react";
import { CopyToClipboard } from 'react-copy-to-clipboard';
import Timestamp from 'react-timestamp';

class CodeListItem extends Component {
    constructor(props) {
        super(props);
        this.state = {
            value: props.snippet.body,
            copied: false,
            timestamp: 0,
        };

    }
    
    componentDidMount() {
        if (this.props.snippet) {
            this.setState({ timestamp: this.props.snippet.timestamp })
        }
    }

    render() {
        return (
            //<li>ID:{this.props.snippet.codeid}<br />body:{this.props.snippet.body}</li>
            <li>
                <CopyToClipboard text={this.state.value}
                    onCopy={() => this.setState({ copied: true })}>
                    <span>ID:{this.props.snippet.codeid} CreatedAt:<Timestamp time={this.state.timestamp} />
                    <br />
                    {this.props.snippet.body}<br/><br/></span>
                </CopyToClipboard>
            </li>

        )
    }
}

export default CodeListItem;