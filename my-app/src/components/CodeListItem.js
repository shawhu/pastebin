import React, { Component } from "react";
import { CopyToClipboard } from 'react-copy-to-clipboard';
import Timestamp from 'react-timestamp';
import AlertContainer from 'react-alert'

class CodeListItem extends Component {
    alertOptions = {
        offset: 14,
        position: 'bottom left',
        theme: 'dark',
        time: 5000,
        transition: 'scale'
      }
     
      
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
                <AlertContainer ref={a => this.msg = a} {...this.alertOptions} />
                <CopyToClipboard text={this.state.value}
                    onCopy={() => {
                        this.setState({ copied: true })
                        this.msg.show(`ID:${this.props.snippet.codeid} copied to the clipboard`, {
                            time: 2000,
                            type: 'success'
                          })
                    }}>
                    <span>ID:{this.props.snippet.codeid} CreatedAt:<Timestamp time={this.state.timestamp} />
                        <pre>
                        {this.props.snippet.body}<br />
                        </pre>
                    </span>
                </CopyToClipboard>
            </li>

        )
    }
}

export default CodeListItem;