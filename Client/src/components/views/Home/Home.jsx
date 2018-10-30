import React, { Component } from 'react';
import Row from 'react-bootstrap/lib/Grid';
import Col from 'react-bootstrap/lib/Grid';
import Header from '../../Header/Header.jsx';
import Info from '../../Info/Info.jsx';
import Menu from '../../Menu/Menu.jsx';
import TextContainer from '../../TextContainer/TextContainer.jsx';
import * as Stats from '../../../js/Stats.js';
import ConstVars from '../../../js/Consts.js';
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

		this.getAllBets = this.getAllBets.bind(this);
		this.setTextItems = this.setTextItems.bind(this);
		this.dismissAlert = this.dismissAlert.bind(this);
	}

	render() {
		return (
		<div className="content" onLoad={this.getAllBets}>
			<Header title={"Logged in as " + window.sessionStorage.getItem('loggedUser')}></Header>
			<Menu disable={this.state.menuDisabled}></Menu>
			<Info alertState={this.state.alertState} alertText={this.state.alertText} dismiss={this.dismissAlert}></Info>
			<Row className="show-grid">
				<Col className="col-md-6 col-xs-12">
					<TextContainer className="textContainer" items={this.state.textItems}></TextContainer>
				</Col>
			</Row>
		</div>
		);
	}

	//gets a list of users bets that have finished. On receiving data, adds data to overviewItems.
	getAllBets(){
		var xmlHttp = new XMLHttpRequest();

		xmlHttp.onreadystatechange =( () => {
				if (xmlHttp.readyState === 4 && xmlHttp.status === 200) {
					console.log(xmlHttp.status);
					this.setTextItems(JSON.parse(xmlHttp.responseText));
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 401) {
					console.log(xmlHttp.status);
					this.setState({
						alertState: xmlHttp.status,
						alertText: "Session error, please login again"
					});
				}

        });
		xmlHttp.open("GET", ConstVars.URI + 'bets?finished=true');
		xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
        xmlHttp.send();
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
