import React, { Component } from 'react';
import PropTypes from 'prop-types';
import store from '../../store';
import Form from 'react-bootstrap/lib/Form';
import FormControl from 'react-bootstrap/lib/FormControl';
import FormGroup from 'react-bootstrap/lib/FormGroup';
import ControlLabel from 'react-bootstrap/lib/ControlLabel';
import Radio from 'react-bootstrap/lib/Radio';
import Confirm from '../Confirm/Confirm.jsx';
import Search from '../Search/Search.jsx';
import Tag from '../Tag/Tag.jsx';
import {betResultToRadioButtonValue, isValidDouble, isValidString} from '../../js/utils.js';
import './Bet.css';

class Bet extends Component{
  constructor(props){
    super(props);

    let bet = props.bet;

    this.state = {
      showAlert: false,
      deleteFromFolder: null,
      name: bet.name,
      bet: bet.bet,
      odd: bet.odd,
      betResult: betResultToRadioButtonValue(bet.bet_won)
    }
  }

  render(){

    return (
      <div>
        <Confirm visible={this.state.showAlert} bsStyle="warning" headerText="Delete bet?" confirmAction={this.deleteBet} cancelAction={this.handleDismiss}/>
        <div className="actionsDiv">
          <i className="imageB trash fas fa-trash-alt fa-2x" onClick={this.onPressedDelete.bind(this, null)}></i>
          <i className="imageB save fas fa-save fa-2x" onClick={this.updateBet.bind(this, null)}></i>
        </div>
        <h2>{this.props.bet.name}</h2>
        <h2>{this.props.bet.datetime}</h2>
        <Form>
          <FormGroup>
            <ControlLabel>Name</ControlLabel>
            <FormControl type="text" value={this.state.name} onChange={this.setValue.bind(this, "name")}/>
            <ControlLabel>Bet</ControlLabel>
            <FormControl type="number" value={this.state.bet} onChange={this.setValue.bind(this, "bet")}/>
            <ControlLabel>Odd</ControlLabel>
            <FormControl type="number" value={this.state.odd} onChange={this.setValue.bind(this, "odd")}/>
            <FormGroup type="radio" onChange={this.setValue.bind(this, "betResult")} value={this.state.betResult}>
              <Radio name="radioGroup" value={-1} inline defaultChecked={this.state.betResult === -1}>Unresolved</Radio>{' '}
              <Radio name="radioGroup" value={1} inline defaultChecked={this.state.betResult === 1}>Won</Radio>{' '}
              <Radio name="radioGroup" value={0} inline defaultChecked={this.state.betResult === 0}>Lost</Radio>
            </FormGroup>
          </FormGroup>
        </Form>
        <div className="tagDiv">
          {this.renderIsInFoldersList()}
        </div>
        <div>
          <Search placeholder="Add folders" data={this.getUnselectedFolders()} onClickResult={this.onPressedAddFolder} />
        </div>
      </div>
    )
  }

  renderIsInFoldersList = () => {
    let i = -1;

    return this.props.foldersOfBet.map(item => {
      i = i + 1;
      return <Tag key={i} value={item} onClick={this.onPressedDelete}/>;
    });
  }

  getUnselectedFolders = () => {
    return this.props.allFolders.filter(
      function(e) {
        return this.indexOf(e) < 0;
      },
      this.props.foldersOfBet
    );
  }

  onPressedDelete = (folder) => {
    if (folder !== null){
      this.setState({
        deleteFromFolder: folder
      }, () => {this.deleteBet()});
    } else {
      this.setState({
        showAlert: true,
        deleteFromFolder: folder
      });
    }
  }

  onPressedAddFolder = (folder) => {
    let data = {
      folders: [folder],
      bet_won: this.props.bet.bet_won,
      odd: this.props.bet.odd,
      bet: this.props.bet.bet
    }

    store.dispatch({type: 'PUT_BET', payload: {
        data: data,
        bet_id: this.props.bet.bet_id
      },
      callback: () => {this.props.updateFolders(this.props.bet.bet_id)}
    });
  }

  handleDismiss = () => {
    this.setState({
      showAlert: false,
      deleteFromFolders: null
    })
  }

  /*
    Updates a bet.
  */
  updateBet = () => {
    const state = this.state;

    if (!isValidString(state.name)){
      store.dispatch({type: 'SET_ALERT_STATUS',
        status: -1,
        message: "Name contains invalid characters"
      });

      return;
    }

    if (!isValidDouble(state.bet) || !isValidDouble(state.odd)){
      store.dispatch({type: 'SET_ALERT_STATUS',
				status: -1,
				message: "Invalid decimal values given"
			});

			return;
    }

    let modifiedBet = {
      name: state.name,
      bet: parseFloat(state.bet),
      odd: parseFloat(state.odd),
      bet_won: parseInt(state.betResult, 10)
    }

    store.dispatch({type: 'PUT_BET', payload: {
        data: modifiedBet,
        bet_id: this.props.bet.bet_id
      },
      showAlert: true
    });
  }

  //Creates a request to delete the selected bet. If any folders are selected, bet is only deleted from selected folders.
  deleteBet = () => {
    let folders = [];
    if (this.state.deleteFromFolder !== null){
      folders = [this.state.deleteFromFolder];
    }
    let callback = undefined;

    if (this.state.deleteFromFolder !== null){
      callback = () => { this.props.updateFolders(this.props.bet.bet_id)};
    }

    store.dispatch({type: 'DELETE_BET', payload: {
        bet_id: this.props.bet.bet_id,
        folders: folders
      },
      callback: callback
    });

    if (callback === undefined){
      this.props.onDelete();
    }
    this.setState({
      showAlert: false,
      deleteFromFolder: null
    });
  }

  setValue = (param, event) => {
    this.setState({
      [param]: event.target.value
    });
  }
};

Bet.propTypes = {
  bet: PropTypes.object.isRequired,
  foldersOfBet: PropTypes.array.isRequired,
  allFolders: PropTypes.array.isRequired,
  onDelete: PropTypes.func.isRequired,
  updateFolders: PropTypes.func.isRequired
};

export default Bet;
