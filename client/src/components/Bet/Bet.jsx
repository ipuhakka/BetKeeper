import React, { Component } from 'react';
import PropTypes from 'prop-types';
import store from '../../store';
import Alert from 'react-bootstrap/lib/Alert';
import Button from 'react-bootstrap/lib/Button';
import Search from '../Search/Search.jsx';
import Tag from '../Tag/Tag.jsx';
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
    }
    else if (!this.props.bet.bet_won && this.props.bet.bet_won !== null){
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
          <i className="imageB fas fa-trash-alt fa-2x" onClick={this.onPressedDelete.bind(this, null)}></i>
        </div>
          <div className="tagDiv">
            {this.renderIsInFoldersList()}
          </div>
          <div>
            <Search placeholder="Add folders" data={this.getUnselectedFolders()} onClickResult={this.onPressedAddFolder} />
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
