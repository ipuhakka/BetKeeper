/*Bet is a component which is shown when a bet is clicked in Bets window.
It shows Bet data and folders which it is in. Bet can be deleted from a folder
by clicking a trash can icon next to folder name. This action needs to be
confirmed before completing.

// TODO: Once backend has been modified so that it's possible
to add folders for a bet, a list of folders where bet isn't should
be displayed as well.*/
import React, { Component } from 'react';
import PropTypes from 'prop-types';
import ListGroup from 'react-bootstrap/lib/ListGroup';
import ListGroupItem from 'react-bootstrap/lib/ListGroupItem';
import './Bet.css';

class Bet extends Component{
  render(){
    let small = "Odd: " + this.props.bet.odd + " Bet: " + this.props.bet.bet;
    let result = "Unresolved";
    if (this.props.bet.bet_won){
      result = "Won";
    } else if (!this.props.bet.bet_won && this.props.bet.bet_won !== null){
      result = "Lost";
    }

    return (
      <div>
        <h2>{this.props.bet.name}</h2>
        <h2>{this.props.bet.datetime}</h2>
        <h4>{small}</h4>
        <div>
          <h4>{result}</h4>
          <i className="fas fa-trash-alt fa-2x"></i>
        </div>
          <ListGroup>{this.renderList()}</ListGroup>
      </div>
    )
  }

  renderList = () => {
    let i = -1;
    return this.props.folders.map(item => {
      i = i + 1;
      return <ListGroupItem key={i}>{item}
              <i className="fas fa-minus-circle fa-2x delete"></i>
            </ListGroupItem>;
    });
  }
};

Bet.propTypes = {
  bet: PropTypes.object.isRequired,
  folders: PropTypes.array.isRequired
};

export default Bet;
