import React, { Component } from 'react';
import logo from './icon.svg';
import './css/App.css';
import Menu from './Menu.jsx';
import Tabs from 'react-bootstrap/lib/Tabs';
import Tab from 'react-bootstrap/lib/Tab';
import AddBets from './AddBets.jsx';
import DeleteBets from './DeleteBets.jsx';

class Bets extends Component{
	constructor(props){
		super(props);
		
		this.state = {
			menuDisabled: [false, true, false, false, false],
		};
		
	}
	
	render(){
		return(
		<div className="App">
			<header className="App-header">
				<img src={logo} className="App-logo" alt="logo" />
				<h1 className="App-title">{"Logged in as " + window.sessionStorage.getItem('loggedUser')}</h1>
			</header>
			<Menu disable={this.state.menuDisabled}></Menu>
			<Tabs defaultActiveKey={1} id="bet-tab">
				<Tab eventKey={1} title={"Add bets"}>
					<AddBets></AddBets>
				</Tab>
				<Tab eventKey={2} title={"Delete bets"}>
					<DeleteBets></DeleteBets>
				</Tab>
			</Tabs>
		</div>);
	}
}

export default Bets;