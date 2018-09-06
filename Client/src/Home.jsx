import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import App from './App.jsx';
import './css/App.css'; 
import logo from './icon.svg';
import AddBets from './AddBets.jsx';
import Menu from './Menu.jsx';

class Home extends Component{
	
	constructor(props){
		super(props);
		
		this.state = {
			menuDisabled: [true, false, false, false, false]
		};
		
		this.logOut = this.logOut.bind(this);
		this.handleSelect = this.handleSelect.bind(this);
	}
	
	render() {
		return (
		<div className="App">
			<header className="App-header">
				<img src={logo} className="App-logo" alt="logo" />
				<h1 className="App-title">{"Logged in as " + window.sessionStorage.getItem('loggedUser')}</h1>
			</header>
			<Menu disable={this.state.menuDisabled}></Menu>
		</div>
		);
	}
	
	handleSelect(key){
		switch (key){
			case 1:
				ReactDOM.render(<AddBets />, document.getElementById('root'));
				break;
			case 4:
				this.logOut();
				break;
			default:
				console.log("Pressed " + key);
				break;
		}
	}
	
	logOut(){
		window.sessionStorage.setItem('loggedUser', null);
		window.sessionStorage.setItem('token', null);
		ReactDOM.render(<App />, document.getElementById('root'));
	}
}

export default Home;