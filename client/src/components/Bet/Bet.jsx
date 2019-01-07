/*Bet is a component which is shown when a bet is clicked in Bets window.
It shows Bet data and folders which it is in. Bet can be deleted from a folder
by clicking a trash can icon next to folder name. This action needs to be
confirmed before completing.

// TODO: Once backend has been modified so that it's possible
to add folders for a bet, a list of folders where bet isn't should
be displayed as well.*/
import React, { Component } from 'react';
import PropTypes from 'prop-types';
import store from '../../store';
import Alert from 'react-bootstrap/lib/Alert';
import Button from 'react-bootstrap/lib/Button';
import ListGroup from 'react-bootstrap/lib/ListGroup';
import ListGroupItem from 'react-bootstrap/lib/ListGroupItem';
import './Bet.css';

class Bet extends Component{
  constructor(props){
    super(props);

    this.state = {
      showAlert: false,
      deleteFromFolder: null
    }
  }

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
        {this.renderAlert()}
        <div>
          <h4>{result}</h4>
          <i className="imageB fas fa-trash-alt fa-2x" onClick={this.onPressedDelete.bind(this, -1)}></i>
        </div>
          <ListGroup>{this.renderIsInFoldersList()}</ListGroup>
          <div>
            <h4>Add to folders:</h4>
            <ListGroup>{this.renderIsNotInFoldersList()}</ListGroup>
          </div>
      </div>
    )
  }

  renderAlert = () => {
    if (this.state.showAlert) {
      return (
        <Alert bsStyle="danger" onDismiss={this.handleDismiss}>
          <h4>{"Delete bet?"}</h4>
          <p>
            <Button bsStyle="danger" onClick={this.deleteBet}>Delete</Button>
            <span> or </span>
            <Button onClick={this.handleDismiss}>Cancel</Button>
          </p>
        </Alert>
      );
    }
    else {
      return null;
    }
  }

  renderIsInFoldersList = () => {
    let i = -1;
    return this.props.foldersOfBet.map(item => {
      i = i + 1;
      return <ListGroupItem key={i}>{item}
              <i className="fas fa-minus-circle fa-2x imageB listItem" onClick={this.onPressedDelete.bind(this, i)}></i>
            </ListGroupItem>;
    });
  }

  renderIsNotInFoldersList = () => {
    let folders = [];
    for (var i = 0; i < this.props.allFolders.length; i++){
      if (!this.props.foldersOfBet.includes(this.props.allFolders[i])){
        folders.push(<ListGroupItem key={i}>{this.props.allFolders[i]}
              <i className="fas fa-plus-circle fa-2x imageB listItem" onClick={this.onPressedAddFolder.bind(this, i)}></i>
            </ListGroupItem>);
      }
    }
    return folders;
  }

  onPressedDelete = (key) => {
    let folder = null;
    if (key !== -1){
      folder = this.props.foldersOfBet[key];
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

  onPressedAddFolder = (key) => {
    let data = {
      folders: [this.props.allFolders[key]],
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
};

Bet.propTypes = {
  bet: PropTypes.object.isRequired,
  foldersOfBet: PropTypes.array.isRequired,
  allFolders: PropTypes.array.isRequired,
  onDelete: PropTypes.func.isRequired,
  updateFolders: PropTypes.func.isRequired
};

export default Bet;
