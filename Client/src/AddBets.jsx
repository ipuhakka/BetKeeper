import React, { Component } from 'react';
import './css/App.css';
import Button from 'react-bootstrap/lib/Button';
import Form from 'react-bootstrap/lib/Form';
import FormControl from 'react-bootstrap/lib/FormControl';
import FormGroup from 'react-bootstrap/lib/FormGroup';
import ControlLabel from 'react-bootstrap/lib/ControlLabel';
import Radio from 'react-bootstrap/lib/Radio';
import ListGroup from 'react-bootstrap/lib/ListGroup';
import ListGroupItem from 'react-bootstrap/lib/ListGroupItem';
import Alert from 'react-bootstrap/lib/Alert';
import Row from 'react-bootstrap/lib/Grid';
import Col from 'react-bootstrap/lib/Grid';
import ConstVars from './js/Consts.js';

class AddBets extends Component {
	constructor(props){
		super(props);

		this.state = {
			selected: [],
			odd: 0.0,
			bet: 0.0,
			name: "",
			betResult: null,
			updateBetResult: null,
			alertState: null,

			selectedBet: -1
		};

		this.setOdd = this.setOdd.bind(this);
		this.setBet = this.setBet.bind(this);
		this.setName = this.setName.bind(this);
		this.setBetResult = this.setBetResult.bind(this);
		this.setUpdateBetResult = this.setUpdateBetResult.bind(this);
		this.addBet = this.addBet.bind(this);
		this.setFoldersList = this.setFoldersList.bind(this);
		this.getAlert = this.getAlert.bind(this);
		this.setAlertState = this.setAlertState.bind(this);
		this.dismissAlert = this.dismissAlert.bind(this);
		this.handleBetListClick = this.handleBetListClick.bind(this);
		this.updateResult = this.updateResult.bind(this);
	}
	
	render(){		
		var items = [];
	
		for (var i = 0; i < this.props.folders.length; i++){
			items.push(<ListGroupItem bsStyle={this.state.selected[i] ?  'info': null} onClick={this.pressedListItem.bind(this, i)} key={i}>{this.props.folders[i]}</ListGroupItem>)
		}
		
		var alert = this.getAlert();
		var unResolved = this.renderBetsList();
		
		return(
			<div className="App">
				<div>{alert}</div>
				<Row className="show-grid">
					<Col className="col-md-6 col-xs-12">
						<Form>
							<FormGroup className = "formMargins">
								<ControlLabel>{"Bet"}</ControlLabel>
								<FormControl 
									type="number"
									value={this.state.bet}
									onChange={this.setBet}
									/>
								<ControlLabel>{"Odd"}</ControlLabel>
								<FormControl 
									type="number"
									value={this.state.odd}
									onChange={this.setOdd}/>
								<ControlLabel>{"Name"}</ControlLabel>
								<FormControl 
									type="text"
									value={this.state.name}
									onChange={this.setName}/>			
							</FormGroup>
							<FormGroup type="radio" className="formMargins" onChange={this.setBetResult} value={this.state.betResult}>
								<Radio name="radioGroup" value={0} inline>Unresolved</Radio>{' '}
								<Radio name="radioGroup" value={1} inline>Won</Radio>{' '}
								<Radio name="radioGroup" value={-1} inline>Lost</Radio>
							</FormGroup>
							<ListGroup className='list'>{items}</ListGroup>
							<Button disabled={this.state.betResult === null} bsStyle="primary" className="button" onClick={this.addBet}>New bet</Button>
						</Form>
					</Col>
					<Col className="col-md-6 col-xs-12">
						<ControlLabel>{"Unresolved bets"}</ControlLabel>
						<ListGroup className='list'>{unResolved}</ListGroup>
						<FormGroup type="radio" className="formMargins" onChange={this.setUpdateBetResult} value={this.state.updateBetResult}>
								<Radio name="radioGroup" value={true} inline>Won</Radio>{' '}
								<Radio name="radioGroup" value={false} inline>Lost</Radio>
						</FormGroup>
						<Button disabled={this.state.updateBetResult === null || this.state.selectedBet === -1} bsStyle="primary" className="button" onClick={this.updateResult}>Update result</Button>
					</Col>
				</Row>
			</div>
		);
	}
	
	renderBetsList(){
		var items = [];
		for (var i = this.props.bets.length - 1; i >= 0; i--){
			items.push(<ListGroupItem header={this.props.bets[i].name + " " + this.props.bets[i].datetime} key={i} onClick={this.handleBetListClick.bind(this, i)} bsStyle={this.state.selectedBet === i ? 'info' : null}>
						{"Odd: " + this.props.bets[i].odd + " Bet: " + this.props.bets[i].bet}</ListGroupItem>)
		}
		return items;
	}
	
	handleBetListClick(key){
		if (key === this.state.selectedBet){
			this.setState({
				selectedBet: -1
			});
		}
		else {
			this.setState({
				selectedBet: key
			});
		}
	}
	
	getAlert(){
		switch(this.state.alertState){
			case "Null result":
				return(<Alert bsStyle="warning" onDismiss={this.dismissAlert}>
							<p>{"Missing result"}</p>
							<Button onClick={this.dismissAlert}>{"Hide"}</Button>
						</Alert>);
			case "Decimal error":
				return(<Alert bsStyle="warning" onDismiss={this.dismissAlert}>
							<p>{"Invalid decimal values given"}</p>
							<Button onClick={this.dismissAlert}>{"Hide"}</Button>
						</Alert>);
			case "Bad request":
				return(<Alert bsStyle="warning" onDismiss={this.dismissAlert}>
							<p>{"Something went wrong with the request"}</p>
							<Button onClick={this.dismissAlert}>{"Hide"}</Button>
						</Alert>);
			case "OK":
				return(<Alert bsStyle="success" onDismiss={this.dismissAlert}>
							<p>{"Bet added successfully"}</p>
							<Button onClick={this.dismissAlert}>{"Hide"}</Button>
						</Alert>);
			case "No content":
				return(<Alert bsStyle="success" onDismiss={this.dismissAlert}>
							<p>{"Bet modified successfully"}</p>
							<Button onClick={this.dismissAlert}>{"Hide"}</Button>
						</Alert>);
			case "Unauthorized":
				return(<Alert bsStyle="warning" onDismiss={this.dismissAlert}>
							<p>{"Unauthorized, please login again"}</p>
							<Button onClick={this.dismissAlert}>{"Hide"}</Button>
						</Alert>);
			case "Not found":
				return(<Alert bsStyle="warning" onDismiss={this.dismissAlert}>
							<p>{"Bet to be deleted was not found"}</p>
							<Button onClick={this.dismissAlert}>{"Hide"}</Button>
						</Alert>);
			case "No bet":
				return(<Alert bsStyle="warning" onDismiss={this.dismissAlert}>
							<p>{"Please select a bet to be modified"}</p>
							<Button onClick={this.dismissAlert}>{"Hide"}</Button>
						</Alert>);
			case "Conflict":
				return(<Alert bsStyle="warning" onDismiss={this.dismissAlert}>
							<p>{"Bet did not belong to user trying to delete it"}</p>
							<Button onClick={this.dismissAlert}>{"Hide"}</Button>
						</Alert>);
			default:
				return;
		}
	}
	
	setAlertState(data){
		this.setState({
			alertState: data
		});
	}
	
	dismissAlert(){
		this.setState({
			alertState: null
		});
	}
	
	//Creates an XMLHttpRequest to add a new bet to database, if bet and odd values are valid. 
	addBet(){		
		if (Number.isNaN(this.state.bet) || Number.isNaN(this.state.odd)){
			this.setAlertState("Decimal error");
			return;
		}
		
		var selectedFolders = []
		for (var i = 0; i < this.props.folders.length; i++){
			if (this.state.selected[i])
				selectedFolders.push(this.props.folders[i]);
		}
		
		var bet_won = "null";
		var date = new Date();
		if (this.state.betResult === -1 || this.state.betResult === '-1'){
			bet_won = false;
		}
		else if (this.state.betResult === 1 || this.state.betResult === '1'){
			bet_won = true;
		}
		var data = {
			bet_won: bet_won,
			odd: this.state.odd,
			bet: this.state.bet,
			name: this.state.name,
			datetime: date,
			folders: selectedFolders
		}
		
		var xmlHttp = new XMLHttpRequest();
		
		xmlHttp.onreadystatechange =( () => {
				if (xmlHttp.readyState === 4 && xmlHttp.status === 200) {
					console.log(xmlHttp.status);
					this.setAlertState("OK");
					this.setState({
						selected: [],
						odd: 0.0,
						bet: 0.0,
						name: ""
					});
					this.props.onUpdate();
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 400) {
					this.setAlertState("Bad request");
					console.log(xmlHttp.status);
					console.log(xmlHttp.responseText);
				}			
				if (xmlHttp.readyState === 4 && xmlHttp.status === 401) {
					this.setAlertState("Unauthorized");
					console.log(xmlHttp.status);
				}		

        });
		xmlHttp.open("POST", ConstVars.URI + "bets/");
		xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
        xmlHttp.send(JSON.stringify(data));
	}
	
	setOdd(e){
		this.setState({
			odd: e.target.value
		});
	}
	
	setBet(e){
		this.setState({
			bet: e.target.value
		});
	}
	
	setName(e){
		this.setState({
			name: e.target.value
		});
	}
	
	setBetResult(e){			
		this.setState({
			betResult: e.target.value
		});
	}
	
	setUpdateBetResult(e){
		this.setState({
			updateBetResult: e.target.value
		});
	}
	
	//Changes unresolved bet to solved. 
	updateResult(){
		if (this.state.selectedBet === -1){
			this.setAlertState("No bet");
			return;
		}
	
		var data = {
			bet_won: this.state.updateBetResult
		}
		
		var xmlHttp = new XMLHttpRequest();
		
		xmlHttp.onreadystatechange =( () => {
				if (xmlHttp.readyState === 4 && xmlHttp.status === 204) {
					console.log(xmlHttp.status);
					this.setAlertState("No content"); //null selections
					this.setFoldersList();
					this.setState({ 
						selectedBet: -1
					});
					this.props.onUpdate();
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 400) {
					this.setAlertState("Bad request");
					console.log(xmlHttp.status);
					console.log(xmlHttp.responseText);
				}			
				if (xmlHttp.readyState === 4 && xmlHttp.status === 401) {
					this.setAlertState("Unauthorized");
					console.log(xmlHttp.status);
				}	
				if (xmlHttp.readyState === 4 && xmlHttp.status === 404) {
					this.setAlertState("Not found");
					console.log(xmlHttp.status);
				}		
				if (xmlHttp.readyState === 4 && xmlHttp.status === 409) {
					this.setAlertState("Conflict");
					console.log(xmlHttp.status);
				}		
        });
		xmlHttp.open("PUT", ConstVars.URI + "bets/" + this.props.bets[this.state.selectedBet].bet_id);
		xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
        xmlHttp.send(JSON.stringify(data));
	}
	
	///init an array of booleans to keep track of selected list items and set the state.
	setFoldersList(){
		var selected = []
		
		for (var i = 0; i < this.props.folders.length; i++){
			selected.push(false);
		}
		
		this.setState({
			selected: selected
		});
	}
	
	setBetsList(){
		this.props.onUpdate();
	}
	
	pressedListItem(i){
		var selected = this.state.selected;		
		selected[i] = !selected[i];
		
		this.setState({
			selected: selected
		});
	}		
}

export default AddBets;