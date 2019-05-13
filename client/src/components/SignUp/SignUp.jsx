import React, { Component } from 'react';
import store from '../../store';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import FormControl from 'react-bootstrap/FormControl';
import FormGroup from 'react-bootstrap/FormGroup';
import {postUser} from '../../js/Requests/Users.js';
import './SignUp.css';

class SignUp extends Component{
	constructor(props){
		super(props);

		this.state = {
			newUsername: "",
			newPassword: "",
			confirmPassword: "",
		};
	}

	render(){
		return (
			<div className="component" onKeyPress={this.checkEnter}>
				<div className="form">
					<h3>Sign up</h3>
					<Form>
						<FormGroup>
							<FormControl
								type="text"
								value={this.state.newUsername}
								placeholder="Enter new username"
								className="formMargins"
								onChange={this.setNewUsername}
							/>
							<FormControl
								type="password"
								placeholder="Enter new password"
								className="formMargins"
								onChange={this.setPassword}
							/>
							<FormControl
								type="password"
								placeholder="Confirm new password"
								onChange={this.setConfirmPassword}
							/>
						</FormGroup>
					</Form>
					<Button variant="primary" className="button" type="submit" onClick={this.signup}>Sign up</Button>
				</div>
			</div>
		);
	}

	checkEnter = (e) => {
		if (e.which === 13){
			this.signup();
		}
	}
	setNewUsername = (e) => {
		this.setState({
			newUsername: e.target.value
		});
	}

	setPassword = (e) => {
		this.setState({
			newPassword: e.target.value
		});
	}

	setConfirmPassword = (e) => {
		this.setState({
			confirmPassword: e.target.value
		});
	}

	signup = async () => {
		if (this.state.newUsername === "" || this.state.newPassword === ""){
			store.dispatch({type: 'SET_ALERT_STATUS',
				status: -1,
				message: "Please input user name, password and confirm password"
			});
			return;
		}

		if (this.state.newPassword !== this.state.confirmPassword){
			store.dispatch({type: 'SET_ALERT_STATUS',
				status: -1,
				message: "Passwords given do not match"
			});
			return;
		}

    try {
			let user = await postUser(this.state.newUsername, this.state.newPassword);
			this.props.requestToken(user.username, user.password);
		} catch (err){
			switch(err){
				case 409:
					store.dispatch({type: 'SET_ALERT_STATUS',
						status: err,
						message: "User with same username already exists"
					});
					break;
				default:
					store.dispatch({type: 'SET_ALERT_STATUS',
						status: err,
						message: "Something went wrong with the request"
					});
					break;
			}
		}
	}
}

export default SignUp;
