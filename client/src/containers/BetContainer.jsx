import React, { Component } from 'react';
import store from '../store';
import Bet from '../components/Bet/Bet.jsx';
import {betResultToRadioButtonValue, isValidDouble, isValidString} from '../js/utils.js';

class BetContainer extends Component{

  render(){
    return (
      <Bet {...this.props} onUpdateBet={this.updateBet} onDelete={this.deleteBet} onAddFolder={this.onPressedAddFolder}/>
    );
  }

  onPressedAddFolder = (folder) => {
    const {props} = this;
    const {bet} = props;

    let data = {
      folders: [folder],
      bet_won: betResultToRadioButtonValue(bet.bet_won),
      odd: bet.odd,
      bet: bet.bet
    }

    store.dispatch({type: 'PUT_BET', payload: {
        data: data,
        bet_id: bet.bet_id
      },
      callback: () => {props.updateFolders(bet.bet_id)}
    });
  }

  //Creates a request to delete the selected bet. If any folders are selected,
  //bet is only deleted from selected folders.
  deleteBet = (folder) => {
    const {props} = this;

    let folders = [];
    let callback = undefined;

    if (folder !== undefined){
      folders = [folder];
      callback = () => { props.updateFolders(props.bet.bet_id)};
    }

    store.dispatch({type: 'DELETE_BET', payload: {
        bet_id: props.bet.bet_id,
        folders: folders
      },
      callback: callback
    });

    if (callback === undefined){
      props.onDelete();
    }
  }

  /*
    Updates a bet.
  */
  updateBet = (bet) => {
    const {props} = this;

    if (!isValidString(bet.name)){
      store.dispatch({type: 'SET_ALERT_STATUS',
        status: -1,
        message: "Name contains invalid characters"
      });

      return;
    }

    if (!isValidDouble(bet.bet) || !isValidDouble(bet.odd)){
      store.dispatch({type: 'SET_ALERT_STATUS',
        status: -1,
        message: "Invalid decimal values given"
      });

      return;
    }

    let modifiedBet = {
      name: bet.name,
      bet: parseFloat(bet.bet),
      odd: parseFloat(bet.odd),
      bet_won: parseInt(bet.betResult, 10)
    }

    store.dispatch({type: 'PUT_BET', payload: {
        data: modifiedBet,
        bet_id: props.bet.bet_id
      },
      showAlert: true
    });
  }
};

export default BetContainer;
