import React, { Component } from 'react';
import PropTypes from 'prop-types';
import Form from 'react-bootstrap/Form';
import FormControl from 'react-bootstrap/FormControl';
import FormGroup from 'react-bootstrap/FormGroup';
import FormCheck from 'react-bootstrap/FormCheck';
import Confirm from '../Confirm/Confirm.jsx';
import Search from '../Search/Search.jsx';
import Tag from '../Tag/Tag.jsx';
import {betResultToRadioButtonValue} from '../../js/utils.js';
import './Bet.css';

class Bet extends Component{
  constructor(props){
    super(props);

    const bet = props.bet;

    this.state = {
      showAlert: false,
      deleteFromFolder: null,
      name: bet.name,
      bet: bet.bet,
      odd: bet.odd,
      betResult: betResultToRadioButtonValue(bet.bet_won)
    }
  }

  componentWillReceiveProps(nextProps){
    const {props} = this;
    const {bet} = props;

    this.setState({
      name: bet.name,
      bet: bet.bet,
      odd: bet.odd,
      betResult: betResultToRadioButtonValue(bet.bet_won)
    });
  }

  render(){
    const {props, state} = this;
    const { bet } = props;

    return (
      <div>
        <Confirm visible={state.showAlert} variant="warning" headerText="Delete bet?" confirmAction={this.onDeleteBet}
          cancelAction={this.handleDismiss}/>
        <div className="actionsDiv">
          <i className="imageB trash fas fa-trash-alt fa-2x" onClick={this.onPressedDelete.bind(this, null)}></i>
          <i className="imageB save fas fa-save fa-2x" onClick={this.updateBet}></i>
        </div>
        <h2>{bet.name}</h2>
        <h2>{bet.datetime}</h2>
        <Form>
          <FormGroup>
            <Form.Label>Name</Form.Label>
            <FormControl type="text" value={state.name} onChange={this.setValue.bind(this, "name")}/>
            <Form.Label>Bet</Form.Label>
            <FormControl type="number" value={state.bet} onChange={this.setValue.bind(this, "bet")}/>
            <Form.Label>Odd</Form.Label>
            <FormControl type="number" value={state.odd} onChange={this.setValue.bind(this, "odd")}/>
            <FormGroup onChange={this.setValue.bind(this, "betResult")} value={state.betResult}>
              <FormCheck name="radioGroup" type="radio" value={-1} label="Unresolved" inline
                defaultChecked={parseInt(state.betResult) === -1}/>
              <FormCheck name="radioGroup" type="radio" value={1} inline label="Won"
                defaultChecked={parseInt(state.betResult) === 1}/>
              <FormCheck name="radioGroup" type="radio" value={0} inline label="Lost"
                defaultChecked={parseInt(state.betResult) === 0}/>
            </FormGroup>
          </FormGroup>
        </Form>
        <div className="tagDiv">
          {this.renderIsInFoldersList()}
        </div>
        <Search placeholder="Add to folder" data={this.getUnselectedFolders()} onClickResult={props.onAddFolder} />
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
    const {props} = this;

    return props.allFolders.filter(
      function(e) {
        return this.indexOf(e) < 0;
      },
      props.foldersOfBet
    );
  }

  handleDismiss = () => {
    this.setState({
      showAlert: false
    })
  }

  /*
    Updates a bet.
  */
  updateBet = () => {
    const {state} = this;

    const modifiedBet = {
      name: state.name,
      bet: state.bet,
      odd: state.odd,
      betResult: state.betResult
    }

    this.props.onUpdateBet(modifiedBet);
  }

  /*
  If folder is specified,
  bet is removed from selected folder.
  Otherwise a confirmation dialog is presented to inform
  that bet will be deleted completely.*/
  onPressedDelete = (folder) => {

    if (folder !== null){
      this.props.onDelete(folder);
    }
    else {
      this.setState({
        showAlert: true
      });
    }
  }

  onDeleteBet = () => {
    this.props.onDelete();

    this.setState({
      showAlert: false
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
