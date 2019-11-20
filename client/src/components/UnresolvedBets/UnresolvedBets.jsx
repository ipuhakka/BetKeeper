import React, { Component } from 'react';
import PropTypes from 'prop-types';
import store from '../../store';
import ListGroup from 'react-bootstrap/ListGroup';
import ListGroupItem from 'react-bootstrap/ListGroupItem';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import FormGroup from 'react-bootstrap/FormGroup';
import enums from '../../js/enums'; 

class UnresolvedBets extends Component{
  constructor(props)
  {
    super(props);

    this.state = {
      selectedBet: -1,
      betResult: null
    };
  }

  render()
  {
    return (
      <div>
        <h4>Unresolved bets</h4>
          <ListGroup>{this.renderBetsList()}</ListGroup>
        <FormGroup 
          className="formMargins"
          value={this.state.betResult}>
            <Form.Check 
              id="check-1" 
              name="radioGroup" 
              type="radio" 
              label='Won' 
              onChange={this.setBetResult} 
              checked={parseInt(this.state.betResult) === enums.betResult.won} 
              value={1} 
              inline/>
            <Form.Check 
              id="check-2" 
              name="radioGroup" 
              type="radio" 
              label='Lost' 
              onChange={this.setBetResult} 
              checked={parseInt(this.state.betResult) === enums.betResult.lost} 
              value={0} 
              inline/>
        </FormGroup>
        <Button 
          disabled={this.state.betResult === null || this.state.selectedBet === -1} 
          variant="primary" 
          className="button" 
          onClick={this.updateResult}>Update result</Button>
      </div>);
  }

  renderBetsList = () => 
  {
    let bets = this.props.bets;
    var items = [];

    for (var i = bets.length - 1; i >= 0; i--)
    {
      items.push(<ListGroupItem action key={i}
        onClick={this.handleListClick.bind(this, i)} 
        variant={this.state.selectedBet === i ? 'info' : null}>
            <div>{bets[i].name + " " + bets[i].playedDate}</div>
            <div className='small-betInfo'>{"Odd: " + bets[i].odd + " Bet: " + bets[i].stake}</div>
            </ListGroupItem>)
    }
    return items;
  }

  handleListClick = (key) =>
  {
    if (key === this.state.selectedBet)
    {
      this.setState({
        selectedBet: -1
      });
    }
    else 
    {
      this.setState({
        selectedBet: key
      });
    }
  }

  setBetResult = (e) => 
  {
  	this.setState({
  		betResult: e.target.value
  	});
  }

  //Changes unresolved bet to solved.
  updateResult = () => 
  {
    if (this.state.selectedBet === -1)
    {
      this.setAlertState("No bet selected", "Invalid input");
      return;
    }
    let result = -1;
    result = parseInt(this.state.betResult, 10);

    var data = {
      betResult: result
    }

    store.dispatch({type: 'PUT_BET', payload: {
        data: data,
        betId: this.props.bets[this.state.selectedBet].betId
      },
      showAlert: true
    });
    
    this.setState({
      betResult: null
    });
  }
};

UnresolvedBets.propTypes = {
  bets: PropTypes.array.isRequired
}

export default UnresolvedBets;
