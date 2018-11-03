import React, { Component } from 'react';
import ReactDOM from 'react-dom';
/*import Row from 'react-bootstrap/lib/Grid';
import Col from 'react-bootstrap/lib/Grid';*/
import Bets from '../Bets/Bets.jsx';
import Folders from '../Folders/Folders.jsx';
import Statistics from '../Statistics/Statistics.jsx';
import Card from '../../Card/Card.jsx';
import Header from '../../Header/Header.jsx';
import Info from '../../Info/Info.jsx';
import Menu from '../../Menu/Menu.jsx';
//import TextContainer from '../../TextContainer/TextContainer.jsx';
import * as Stats from '../../../js/Stats.js';
import {getFinishedBets} from '../../../js/Requests/Bets.js';
import './Home.css';

class Home extends Component{

	constructor(props){
		super(props);

		this.state = {
			menuDisabled: [true, false, false, false, false],
			textItems: ["Bets won/played:", "Money won/played:", "Last bet played:"],
			alertState: null,
			alertText: ""
		};

		this.handleGetAllBets = this.handleGetAllBets.bind(this);
		this.setTextItems = this.setTextItems.bind(this);
		this.dismissAlert = this.dismissAlert.bind(this);
	}

	changePage = (key) => {
		switch (key){
			case 0:
				ReactDOM.render(<Statistics />, document.getElementById('root'));
				break;
			case 1:
				ReactDOM.render(<Bets/>, document.getElementById('root'));
				break;
			case 2:
				ReactDOM.render(<Folders />, document.getElementById('root'));
				break;
			default:
				break;
		}
	}

	cardGrid = () => {
		return (
			<div className="grid">
				<Card onClick={() => {this.changePage(0)}} image="fas fa-chart-bar fa-5x" title="Statistics" text="See how your bets have gone" data={this.props.stats}></Card>
				<Card onClick={() => {this.changePage(1)}} image="fas fa-edit fa-5x" title="Bets" text="Add and delete bets, update unresoved bets"></Card>
				<Card onClick={() => {this.changePage(2)}} image="fas fa-folder fa-5x" title="Folders" text="Create and delete folders"></Card>
			</div>
		);
	}

	render() {
		return (
		<div className="content" onLoad={() => {getFinishedBets(this.handleGetAllBets);}}>
			<Header title={"Logged in as " + window.sessionStorage.getItem('loggedUser')}></Header>
			<Menu disable={this.state.menuDisabled}></Menu>
			<Info alertState={this.state.alertState} alertText={this.state.alertText} dismiss={this.dismissAlert}></Info>
			<div>{this.cardGrid()}</div>
		</div>
		);
	}

	handleGetAllBets(status, data){
		if (status === 200){
			this.setTextItems(JSON.parse(data));
		}
		else if (status === 401){
			this.setState({
				alertState: status,
				alertText: "Session error, please login again"
			});
		}
	}

	setTextItems(betData){
		var textItems = [];
		textItems.push("Won bets: " + Stats.wonBets(betData) + "/" + Stats.playedBets(betData));
		textItems.push("Money won/played: " + Stats.moneyWon(betData) + "/" + Stats.moneyPlayed(betData));
		textItems.push("Last played bet: " + betData[betData.length - 1]["datetime"]);

		this.setState({
			textItems: textItems
		});
	}

	dismissAlert(){
		this.setState({
			alertState: null,
			alertText: ""
		});
	}

}

export default Home;
