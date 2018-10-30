import React, { Component } from 'react';
import Alert from 'react-bootstrap/lib/Alert';
import Button from 'react-bootstrap/lib/Button';

class Info extends Component{
	constructor(props){
		super(props);

		this.renderAlert = this.renderAlert.bind(this);
	}

	render(){
		var alert = this.renderAlert();
		return(<div>{alert}</div>);
	}

	//This function uses a switch to return a type of alert or null. Switch is prop 'alertState'. Its values are statuscodes of
	//http-requests. Any other value returns no alert element.
	renderAlert(){
		switch(this.props.alertState){
			case 200:
				return(<Alert bsStyle="success" onDismiss={this.props.dismiss}>
						<p>{this.props.alertText}</p>
						<Button onClick={this.props.dismiss}>{"Hide"}</Button>
						</Alert>);
			case 201:
				return(<Alert bsStyle="success" onDismiss={this.props.dismiss}>
						<p>{this.props.alertText}</p>
						<Button onClick={this.props.dismiss}>{"Hide"}</Button>
						</Alert>);
			case 400:
				return(<Alert bsStyle="danger" onDismiss={this.dismissAddAlert}><p>{this.props.alertText}</p>
						<Button onClick={this.props.dismiss}>{"Hide"}</Button></Alert>);
			case 401:
				return(<Alert bsStyle="danger" onDismiss={this.dismissAddAlert}><p>{this.props.alertText}</p>
						<Button onClick={this.props.dismiss}>{"Hide"}</Button></Alert>);
			case 404:
				return (<Alert bsStyle="danger" onDismiss={this.props.dismiss}><p>{this.props.alertText}</p>
						<Button onClick={this.props.dismiss}>{"Hide"}</Button></Alert>);
			case 409:
				return (<Alert bsStyle="danger" onDismiss={this.props.dismiss}><p>{this.props.alertText}</p>
						<Button onClick={this.props.dismiss}>{"Hide"}</Button></Alert>);
			case "Null result":
				return(<Alert bsStyle="warning" onDismiss={this.props.dismiss}>
							<p>{this.props.alertText}</p>
							<Button onClick={this.props.dismiss}>{"Hide"}</Button>
						</Alert>);
			case "Invalid input":
				return(<Alert bsStyle="warning" onDismiss={this.props.dismiss}>
							<p>{this.props.alertText}</p>
							<Button onClick={this.props.dismiss}>{"Hide"}</Button>
						</Alert>);
			default:
				return;
		}
	}

}

export default Info
