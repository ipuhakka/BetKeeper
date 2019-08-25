import React, { Component } from 'react';
import PropTypes from 'prop-types';
import store from '../../store';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import FormControl from 'react-bootstrap/FormControl';
import FormGroup from 'react-bootstrap/FormGroup';
import Modal from 'react-bootstrap/Modal';
import Tag from '../Tag/Tag.jsx';
import Search from '../Search/Search.jsx';
import {isValidDouble, isValidString} from '../../js/utils.js';
import './AddBet.css';

class AddBet extends Component
{
  constructor(props)
  {
    super(props);

    this.state = {
      selected: [],
      bet: 0.0,
      odd: 0.0,
      name: "",
      betResult: -1,
      dragging: false,
      draggedOutOfModal: false
    };
  }

  render()
  {
    const {state} = this;

    return (
      <Modal 
        onMouseOut={this.handleMouseOut} 
        onMouseDown={this.handleMouseDown} 
        onMouseUp={this.handleMouseUp.bind(this)} 
        show={this.props.show} 
        onHide={this.hideModal}>
        <Modal.Header closeButton>
          <Modal.Title>{"Add bet"}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            <FormGroup className = "formMargins">
              <Form.Label>{"Bet"}</Form.Label>
              <FormControl
                type="number"
                value={state.bet}
                onChange={this.setValue.bind(this, "bet")}
                />
              <Form.Label>{"Odd"}</Form.Label>
              <FormControl
                type="number"
                value={this.state.odd}
                onChange={this.setValue.bind(this, "odd")}/>
              <Form.Label>{"Name"}</Form.Label>
              <FormControl
                type="text"
                value={this.state.name}
                onChange={this.setValue.bind(this, "name")}/>
            </FormGroup>
            <FormGroup id="checkResult" className="formMargins" onChange={this.setValue.bind(this, "betResult")} value={this.state.betResult}>
              <Form.Check name="betResult" id="check-1" value={-1} inline type="radio" label='Unresolved' defaultChecked={parseInt(this.state.betResult) === -1} />
              <Form.Check name="betResult" id="check-2" value={1} inline type="radio" label='Won' defaultChecked={parseInt(this.state.betResult) === 1} />
              <Form.Check name="betResult" id="check-3" value={0} inline type="radio" label='Lost' defaultChecked={parseInt(this.state.betResult) === 0} />
            </FormGroup>
          </Form>
          <div className="tagDiv">
            {this.renderTags()}
          </div>
          <Search 
            clearOnClick={true} 
            data={this.filterFolderList()} 
            onClickResult={this.selectFolder} 
            placeholder="Search folders"/>
          <Button disabled={state.betResult === null} variant="primary" className="button" onClick={this.addBet}>New bet</Button>
        </Modal.Body>
      </Modal>
    );
  }

  renderTags = () => 
  {
    let i = -1;
      return this.state.selected.map(folder => {
        i = i + 1;
        return (<Tag key={i} value={folder} onClick={this.removeFolderFromSelected}/>);
      });
  }

  /* Sets dragging to true to prevent closing modal on dragging */
  handleMouseDown = (e) => 
  {
    this.setState({
      dragging: true,
      draggedOutOfModal: false
    });
  }

  /* Sets dragging to false*/
  handleMouseUp = () => 
  {
    this.setState({
      dragging: false
    })
  }

  /* Sets draggedOutOfModal to prevent hiding the modal on drag.*/
  handleMouseOut = () => 
  {
    let dragging = this.state.dragging;
    this.setState({
      draggedOutOfModal: dragging
    })
  }

  /* Filters props.folders by removing selected folders.*/
  filterFolderList = () => 
  {
    const {state, props} = this;

    return props.folders.filter(folder =>
      !state.selected.some(selectedFolder =>
        selectedFolder === folder));
  }

  hideModal = () => {

    if (!this.state.draggedOutOfModal)
    {
      this.setState({
        selected: []
      });

      this.props.hide();
    }
  }

  removeFolderFromSelected = (folder) => 
  {
    let selectedFolders = this.state.selected;

    selectedFolders.splice(selectedFolders.indexOf(folder), 1);

    this.setState({
      selected: selectedFolders
    });
  }

  selectFolder = (folder) => {
    var selectedFolders = this.state.selected;

    if (!this.state.selected.includes(folder))
    {
      selectedFolders.push(folder);

      this.setState({
        selected: selectedFolders
      });
    }
  }

  setValue = (param, e) => 
  {

    this.setState({
      [param]: e.target.value
    });
  }

  //Creates a new bet to database, if bet and odd values are valid and a result for a bet is set.
  addBet = () => 
  {

    if (!isValidString(this.state.name))
    {
      store.dispatch({type: 'SET_ALERT_STATUS',
        status: -1,
        message: "Name contains invalid characters"
      });

      return;
    }

    if (!isValidDouble(this.state.bet) || !isValidDouble(this.state.odd))
    {
      store.dispatch({type: 'SET_ALERT_STATUS',
        status: -1,
        message: "Invalid decimal values given"
      });

      return;
    }

		var selectedFolders = this.state.selected;

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

    this.hideModal();
	}

}

AddBet.propTypes = {
  show: PropTypes.bool.isRequired,
  hide: PropTypes.func.isRequired,
  folders: PropTypes.array.isRequired
};

export default AddBet;
