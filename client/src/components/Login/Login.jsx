import React, { Component } from 'react';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import FormControl from 'react-bootstrap/FormControl';
import FormGroup from 'react-bootstrap/FormGroup';
import './Login.css';

class Login extends Component {
	constructor(props){
		super(props);

		this.state = {
			username: "",
			password: ""
		};
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
							<FormControl
								onChange={this.setPassword}
								type="password"
								placeholder="Enter password">
							</FormControl>
						</FormGroup>
					</Form>
					<Button variant="primary" className="button" type="submit" onClick={() => this.props.requestToken(this.state.username, this.state.password)}>Login</Button>
				</div>
			</div>
		);
	}

	setUsername = (e) => {
		this.setState({
			username: e.target.value
		});
	}

	setPassword = (e) => {
		this.setState({
			password: e.target.value
		});
	}

	checkEnter = (e) => {
		if (e.which === 13){ //login
			this.props.requestToken(this.state.username, this.state.password);
		}
	}
}

export default Login;
