import React, { Component } from 'react';
import Button from 'react-bootstrap/lib/Button';
import Form from 'react-bootstrap/lib/Form';
import FormControl from 'react-bootstrap/lib/FormControl';
import FormGroup from 'react-bootstrap/lib/FormGroup';
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
					<Button bsStyle="primary" className="button" type="submit" onClick={this.signup}>Sign up</Button>
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

	signup = () => {
		if (this.state.newUsername === "" || this.state.newPassword === ""){
			this.props.alert(-1, "Please input user name, password and confirm password");
			return;
		}

		if (this.state.newPassword !== this.state.confirmPassword){
			this.props.alert(-1, "Passwords given do not match");
			return;
		}

    postUser(this.state.newUsername, this.state.newPassword, this.handleSignUp);
	}

  handleSignUp = (status, data) => {
    switch(status){
      case 201:
        this.props.requestToken(data.username, data.password);
        break;
      case 409:
        this.props.alert(status, "User with same username already exists");
        break;
      default:
        this.props.alert(status, "Something went wrong with the request");
        break;
    }
  }
}

export default SignUp;
