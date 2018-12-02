import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import Header from './components/Header/Header.jsx';
import Home from './views/Home/Home.jsx';
import Info from './components/Info/Info.jsx';
import Login from './components/Login/Login.jsx';
import SignUp from './components/SignUp/SignUp.jsx';
import {postToken, getToken} from './js/Requests/Token.js';
import './App.css';

class App extends Component {
	constructor(props){
		super(props);

		this.state = {
			alertState: null
		};
	}

	render() {
		return (
		<div className="App" onLoad={this.checkToken}>
			<Header title={"Welcome to Betkeeper"}></Header>
			<Info alertState={this.state.alertState} alertText={this.state.alertText} dismiss={this.dismissAlert}></Info>
			<Login requestToken={this.requestToken}></Login>
			<p>Or</p>
			<SignUp requestToken={this.requestToken} alert={this.setAlertState}></SignUp>
		</div>
		);
	}

    //Checks if sessionStorage contains a valid token. If does, goes to user main page as logged user.
	checkToken = () => {
		if (sessionStorage.getItem('token') != null && sessionStorage.getItem('token').toString() !== 'null'){
			getToken(sessionStorage.getItem('token'), sessionStorage.getItem('loggedUserID'), this.handleGetToken);
		}
	}

	handleGetToken = (status) => {
		if (status === 200){
			ReactDOM.render(<Home />, document.getElementById('root'));
		}
	}

	//Makes a post request to resource at URI/token. On success, sets the token from response, and user inputted username
	//to sessionStorage and changes html page.
	requestToken = (user, passwd) => {
		if (user === "" || passwd === ""){
			this.setAlertState("missing inputs");
			return;
		}

		postToken(user, passwd, this.handleReceiveToken);
	}

	handleReceiveToken = (status, token, user_id, user) => {
		switch(status){
			case 200:
				window.sessionStorage.setItem('token', token);
				window.sessionStorage.setItem('loggedUser', user);
				window.sessionStorage.setItem('loggedUserID', user_id);
				ReactDOM.render(<Home />, document.getElementById('root'));
				break;
			case 401:
				this.setAlertState(401, "Username and password don't match");
				break;
			default:
				this.setAlertState(-1, "Something went wrong with the request");
				break;
		}
	}

	dismissAlert = () => {
		this.setState({
			alertState: null
		});
	}

	setAlertState = (state, text) => {
		this.setState({
			alertState: state,
			alertText: text
		});
	}
}


export default App;
