import React, { Component } from 'react';
import Button from 'react-bootstrap/lib/Button';
import Form from 'react-bootstrap/lib/Form';
import FormControl from 'react-bootstrap/lib/FormControl';
import FormGroup from 'react-bootstrap/lib/FormGroup';
import MaskedInput from '../MaskedInput/MaskedInput.jsx';
import './Login.css';

class Login extends Component {
	constructor(props){
		super(props);

		this.state = {
			username: "",
			password: ""
		};

		this.setUsername = this.setUsername.bind(this);
		this.setPassword = this.setPassword.bind(this);
		this.checkEnter = this.checkEnter.bind(this);
	}

	render(){
		return (
			<div className="component" onKeyPress={this.checkEnter}>
				<div className="form">
					<h3>Login</h3>
					<Form>
						<FormGroup>
							<FormControl
								type="text"
								value={this.state.username}
								placeholder="Enter username"
								className = "formMargins"
								onChange={this.setUsername}
							/>
							<MaskedInput
								onChange={this.setPassword}
								type="text"
								placeholder="Enter password">
							</MaskedInput>
						</FormGroup>
					</Form>
					<Button bsStyle="primary" className="button" type="submit" onClick={() => this.props.requestToken(this.state.username, this.state.password)}>Login</Button>
				</div>
			</div>
		);
	}

	setUsername(e){
		this.setState({
			username: e.target.value
		});
	}

	setPassword(psw){
		this.setState({
			password: psw
		});
	}

	checkEnter(e){
		if (e.which === 13){ //login
			this.props.requestToken(this.state.username, this.state.password);
		}
	}
}

export default Login;
