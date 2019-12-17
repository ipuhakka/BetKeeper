import React, { Component } from 'react';
import PropTypes from 'prop-types';
import store from '../../store';
import _ from 'lodash';
import ListGroup from 'react-bootstrap/ListGroup';
import ListGroupItem from 'react-bootstrap/ListGroupItem';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import FormGroup from 'react-bootstrap/FormGroup';
import { putBets } from '../../actions/betsActions';
import enums from '../../js/enums'; 

class UnresolvedBets extends Component{
  constructor(props)
  {
    super(props);

    this.state = {
      selectedBet: -1,
      selectedBetKeys: [],
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
          disabled={this.state.betResult === null 
            || this.state.selectedBetKeys.length < 1} 
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
        variant={_.includes(this.state.selectedBetKeys, i) ? 'info' : null}>
            <div>{bets[i].name + " " + bets[i].playedDate}</div>
            <div className='small-betInfo'>{"Odd: " + bets[i].odd + " Bet: " + bets[i].stake}</div>
            </ListGroupItem>)
    }
    return items;
  }

  handleListClick = (key) =>
  {
    const { selectedBetKeys } = this.state;
    
    let updatedBetKeys = [];

    if (_.includes(selectedBetKeys, key))
    {
      updatedBetKeys = _.filter(selectedBetKeys, betKey =>
        betKey !== key);
    }
    else 
    {
      updatedBetKeys = _.concat(selectedBetKeys, [key]);
    }

    this.setState({
      selectedBetKeys: updatedBetKeys
    });
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
    const { selectedBetKeys } = this.state;
    const { bets } = this.props;

    let result = -1;
    result = parseInt(this.state.betResult, 10);

    var data = {
      betResult: result
    }

    const betIds = _.map(selectedBetKeys, betKey =>
      {
        return bets[betKey].betId;
      });

    store.dispatch(putBets(betIds, data));
    
    this.setState({
      betResult: null,
      selectedBetKeys: []
    });
  }
};

UnresolvedBets.propTypes = {
  bets: PropTypes.array.isRequired
}

export default UnresolvedBets;
