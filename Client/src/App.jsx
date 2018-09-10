import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import logo from './icon.svg';
import Home from './Home.jsx';
import './css/App.css';
import Button from 'react-bootstrap/lib/Button';
import Form from 'react-bootstrap/lib/Form';
import FormControl from 'react-bootstrap/lib/FormControl';
import FormGroup from 'react-bootstrap/lib/FormGroup';
import ConstVars from './Consts.js';
import MaskedInput from './MaskedInput.jsx';
import Alert from 'react-bootstrap/lib/Alert';

class App extends Component {
	constructor(props){
		super(props);		
		
		this.state = { 
			alertState: null
		};
		
		this.requestToken = this.requestToken.bind(this);
		this.checkToken = this.checkToken.bind(this);
		this.getAlert = this.getAlert.bind(this);
		this.setAlertState = this.setAlertState.bind(this);
		this.dismissAlert = this.dismissAlert.bind(this);
	}
	
	render() {
		var alert = this.getAlert();
		
		return (
		<div className="App" onLoad={this.checkToken}>
			<header className="App-header">
				<img src={logo} className="App-logo" alt="logo" />
				<h1 className="App-title">Welcome to Betkeeper</h1>
			</header>
			<div>{alert}</div>
			<Login requestToken={this.requestToken}></Login>
			<p>Or</p>
			<SignUp requestToken={this.requestToken} alert={this.setAlertState}></SignUp>
		</div>
		);
	}
  
    //Checks if sessionStorage contains a valid token. If does, goes to user main page as logged user.
	checkToken(){
		if (sessionStorage.getItem('token') != null && sessionStorage.getItem('token').toString() !== 'null'){
			var xmlHttp = new XMLHttpRequest();
			xmlHttp.onreadystatechange =( () => {
				if (xmlHttp.readyState === 4 && xmlHttp.status === 200) {
					console.log(xmlHttp.status);
					ReactDOM.render(<Home />, document.getElementById('root'));		
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 404) {
					console.log(xmlHttp.status);
					this.setAlertState("Not found");
				}


			});
			xmlHttp.open("GET", ConstVars.URI + "token/?token=" + sessionStorage.getItem('token'));
			xmlHttp.setRequestHeader('Content-Type', 'application/json');
			xmlHttp.send();
		}
	}
  
	//Makes a post request to resource at URI/token. On success, sets the token from response, and user inputted username
	//to sessionStorage and changes html page.
	requestToken(user, passwd){
		if (user === "" || passwd === ""){
			window.alert("Please insert a username and a password");
			return;
		}
		
		var data = {
			username: user,
			password: passwd
		};
		
		var xmlHttp = new XMLHttpRequest();
		
		xmlHttp.onreadystatechange =( () => {
				if (xmlHttp.readyState === 4 && xmlHttp.status === 200) {
					console.log(xmlHttp.status);
					window.sessionStorage.setItem('token', JSON.parse(xmlHttp.responseText).token);
					window.sessionStorage.setItem('loggedUser', user);
					ReactDOM.render(<Home />, document.getElementById('root'));		
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 400) {
					console.log(xmlHttp.status);
					this.setAlertState("Bad request");
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 401) { //username and password don't match
					console.log(xmlHttp.status);
					this.setAlertState("Unauthorized");
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 409) { //user already logged in, switch to homepage.
					console.log(xmlHttp.status);
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 415) {
					console.log(xmlHttp.status);
					this.setAlertState("Content-Type");
				}			

        });
		xmlHttp.open("POST", ConstVars.URI + "token");
		console.log(ConstVars.URI + "token");
		xmlHttp.setRequestHeader('Content-Type', 'application/json');
        xmlHttp.send(JSON.stringify(data));
	}
	
	dismissAlert(){
		this.setState({
			alertState: null
		});
	}
	
	getAlert(){
		switch(this.state.alertState){
			case "Bad request":
				return (<Alert onDismiss={this.dismissAlert} bsStyle={"warning"}>
							<p>{"Something went wrong with the request"}</p>
							<Button onClick={this.dismissAlert}>{"Hide"}</Button>
						</Alert>);
			case "Unauthorized":
				return (<Alert onDismiss={this.dismissAlert} bsStyle={"warning"}>
							<p>{"Password and username don't match"}</p>
							<Button onClick={this.dismissAlert}>{"Hide"}</Button>
						</Alert>);
			case "Conflict":
				return (<Alert onDismiss={this.dismissAlert} bsStyle={"warning"}>
							<p>{"Username already in use"}</p>
							<Button onClick={this.dismissAlert}>{"Hide"}</Button>
						</Alert>);
			case "Content-Type":
				return (<Alert onDismiss={this.dismissAlert} bsStyle={"warning"}>
							<p>{"Request was missing a header"}</p>
							<Button onClick={this.dismissAlert}>{"Hide"}</Button>
						</Alert>);
			case "missing inputs":
				return (<Alert onDismiss={this.dismissAlert} bsStyle={"warning"}>
							<p>{"Please input both username and password"}</p>
							<Button onClick={this.dismissAlert}>{"Hide"}</Button>
						</Alert>);
			case "invalid match":
				return (<Alert onDismiss={this.dismissAlert} bsStyle={"warning"}>
							<p>{"Password didn't match confirmed password"}</p>
							<Button onClick={this.dismissAlert}>{"Hide"}</Button>
						</Alert>);
			case "Not found":
				return (<Alert onDismiss={this.dismissAlert} bsStyle={"warning"}>
							<p>{"Session expired, please login again"}</p>
							<Button onClick={this.dismissAlert}>{"Hide"}</Button>
						</Alert>);
			default:
				return;
		}
	}
	
	setAlertState(state){
		this.setState({
			alertState: state
		});
	}
}

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

class SignUp extends Component{
	constructor(props){
		super(props);
		
		this.state = {
			newUsername: "",
			newPassword: "",
			confirmPassword: "",
		};
			
		this.setNewUsername = this.setNewUsername.bind(this);
		this.setPassword = this.setPassword.bind(this);
		this.setConfirmPassword = this.setConfirmPassword.bind(this);
		this.signup = this.signup.bind(this);
		this.checkEnter = this.checkEnter.bind(this);
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
							<MaskedInput	
								type="text"
								placeholder="Enter new password"							
								className="formMargins"
								onChange={this.setPassword}
							/>
							<MaskedInput
								type="text"
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
	
	checkEnter(e){
		if (e.which === 13){ 
			this.signup();
		}
	}
	setNewUsername(e){
		this.setState({
			newUsername: e.target.value
		});
	}
	
	setPassword(psw) {
		this.setState({
			newPassword: psw
		});
	}
	
	setConfirmPassword(psw){
		this.setState({
			confirmPassword: psw
		});
	}
	
	//Creates an XMLHttpRequest to post a new user. on success asks for a token from the api.
	signup(){
		if (this.state.newUsername === "" || this.state.newPassword === ""){
			this.props.alert("missing inputs");
			return;
		}
		
		if (this.state.newPassword !== this.state.confirmPassword){
			this.props.alert("invalid match");
			return;
		}
	
		var data = {
			username: this.state.newUsername,
			password: this.state.newPassword
		};
		
		var xmlHttp = new XMLHttpRequest();
		
		xmlHttp.onreadystatechange =( () => {
				if (xmlHttp.readyState === 4 && xmlHttp.status === 201) {
					console.log(xmlHttp.status);
					this.props.requestToken(data.username, data.password);
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 400) {
					console.log(xmlHttp.status);
					this.props.alert('Bad request');
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 409) {
					console.log(xmlHttp.status);
					this.props.alert('Conflict');
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 415) {
					console.log(xmlHttp.status);
				}			

        });
		xmlHttp.open("POST", ConstVars.URI + "user/");
		xmlHttp.setRequestHeader('Content-Type', 'application/json');
        xmlHttp.send(JSON.stringify(data));
	}

}


export default App;
