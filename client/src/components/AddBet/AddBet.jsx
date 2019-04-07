import React, { Component } from 'react';
import PropTypes from 'prop-types';
import store from '../../store';
import Button from 'react-bootstrap/lib/Button';
import ControlLabel from 'react-bootstrap/lib/ControlLabel';
import Form from 'react-bootstrap/lib/Form';
import FormControl from 'react-bootstrap/lib/FormControl';
import FormGroup from 'react-bootstrap/lib/FormGroup';
import ListGroup from 'react-bootstrap/lib/ListGroup';
import ListGroupItem from 'react-bootstrap/lib/ListGroupItem';
import Radio from 'react-bootstrap/lib/Radio';
import Modal from 'react-bootstrap/lib/Modal';
import Search from '../Search/Search.jsx';

class AddBet extends Component{
  constructor(props){
    super(props);

    this.state = {
      selected: [],
      bet: 0.0,
      odd: 0.0,
      name: "",
      betResult: null
    };
  }

  render(){
    var items = [];
		for (var i = 0; i < this.props.folders.length; i++){
			items.push(<ListGroupItem bsStyle={this.state.selected[i] ?  'info': null} onClick={this.pressedListItem.bind(this, i)} key={i}>{this.props.folders[i]}</ListGroupItem>)
		}

    return (
      <Modal show={this.props.show} onHide={this.props.hide}>
        <Modal.Header closeButton>
          <Modal.Title>{"Add bet"}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            <FormGroup className = "formMargins">
              <ControlLabel>{"Bet"}</ControlLabel>
              <FormControl
                type="number"
                value={this.state.bet}
                onChange={this.setValue.bind(this, "bet")}
                />
              <ControlLabel>{"Odd"}</ControlLabel>
              <FormControl
                type="number"
                value={this.state.odd}
                onChange={this.setValue.bind(this, "odd")}/>
              <ControlLabel>{"Name"}</ControlLabel>
              <FormControl
                type="text"
                value={this.state.name}
                onChange={this.setValue.bind(this, "name")}/>
            </FormGroup>
            <FormGroup type="radio" className="formMargins" onChange={this.setValue.bind(this, "betResult")} value={this.state.betResult}>
              <Radio name="radioGroup" value={-1} inline defaultChecked={this.state.betResult === "-1"}>Unresolved</Radio>{' '}
              <Radio name="radioGroup" value={1} inline defaultChecked={this.state.betResult === "1"}>Won</Radio>{' '}
              <Radio name="radioGroup" value={0} inline defaultChecked={this.state.betResult === "0"}>Lost</Radio>
            </FormGroup>
          </Form>
          <Search data={this.props.folders} onClickResult={this.selectFolder} placeholder="Search folders"/>
          <Button disabled={this.state.betResult === null} bsStyle="primary" className="button" onClick={this.addBet}>New bet</Button>
        </Modal.Body>
      </Modal>
    );
  }

  selectFolder = (folder) => {
    console.log("Pressed: " + JSON.stringify(folder));
  }

  pressedListItem = (i) => {
    var selected = this.state.selected;
    selected[i] = !selected[i];

    this.setState({
      selected: selected
    });
  }

  setValue = (param, e) => {
    this.setState({
      [param]: e.target.value
    });
  }

  //Creates a new bet to database, if bet and odd values are valid and a result for a bet is set.
	addBet = () => {
		if (Number.isNaN(this.state.bet) || Number.isNaN(this.state.odd)){
			this.setAlertState("Decimal given were in invalid format", "Invalid input");
			return;
		}

		var selectedFolders = []
		for (var i = 0; i < this.props.folders.length; i++){
			if (this.state.selected[i])
				selectedFolders.push(this.props.folders[i]);
		}

		var data = {
			bet_won: parseInt(this.state.betResult, 10),
			odd: parseFloat(this.state.odd),
			bet: parseFloat(this.state.bet),
			name: this.state.name,
			folders: selectedFolders
		}
		store.dispatch({type: 'POST_BET', payload: {
				bet: data
			}
		});
		this.setState({
      bet: 0.0,
      odd: 0.0,
      name: "",
      betResult: null
    });
    this.props.hide();
	}

}

AddBet.propTypes = {
  show: PropTypes.bool.isRequired,
  hide: PropTypes.func.isRequired,
  folders: PropTypes.array.isRequired
};

export default AddBet;
