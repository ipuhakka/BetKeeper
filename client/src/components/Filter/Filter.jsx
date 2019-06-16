import React, { Component } from 'react';
import PropTypes from 'prop-types';
// import ControlLabel from 'react-bootstrap/ControlLabel';
// import FormControl from 'react-bootstrap/FormControl';
// import Checkbox from 'react-bootstrap/Checkbox';
import Form from 'react-bootstrap/Form';
import Dropdown from '../Dropdown/Dropdown.jsx';
import './Filter.css';
import {getFilterOptions, filterList} from '../../js/filter.js';

const selections =
  [
    'Between',
    'Over',
    'Under'
  ];

/*
TODO: Add redux handling for activeBetList &
filter it in reducer.
*/
class Filter extends Component{
  constructor(props){
    super(props);

    this.state = {
      selectedfilterOptionKey: 1,
      higherThan: 0,
      lowerThan: 100,
      singleNumericFilterValue: 0,
      stringFilter: '',
      filterOn: false
    }
  }

  render(){
    const {props, state} = this;
    let filterControls;

    if (props.type === "number"){
      filterControls = this.renderNumericFilter();
    }
    else if (props.type === "text"){
      filterControls = this.renderTextFilter();
    }
    else {
      return null;
    }

    return(
        <div className={state.filterOn ? 'filter checked' : 'filter unchecked'}>
            <Form.Check className="small" onChange={this.toggleChecked}/>
            <div className="small">
            <Dropdown defaultKey={1} stateKey={"selectedfilterOptionKey"} id={1}
                data={selections} title="Bet" onUpdate={this.updateSelection}/>
            </div>
            {filterControls}
        </div>);
  }

  renderNumericFilter = () => {
    const {state} = this;

    if (state.selectedfilterOptionKey === 0){
      return <div>
                <Form.Control disabled={!state.filterOn} className="small" type="number" value={state.higherThan}
                  onChange={this.setValue.bind(this, "higherThan")}/>
                <Form.Control disabled={!state.filterOn} className="small" type="number" value={state.lowerThan}
                  onChange={this.setValue.bind(this, "lowerThan")}/>
              </div>;
    }
    else {
      return <Form.Control disabled={!state.filterOn} className="large" type="number" value={state.singleNumericFilterValue}
                onChange={this.setValue.bind(this, "singleNumericFilterValue")}/>;
    }

  }

  renderTextFilter = () => {
    const {props, state} = this;

    return(
        <div>
          <Form.Label className="small">{props.label}</Form.Label>
          <Form.Control disabled={!state.filterOn} className="large" type="text" value={state.stringFilter}
           onChange={this.setValue.bind(this, "stringFilter")}/>
        </div>);
  }

  /*
  * Returns filtered array.
  */
  onUpdate = () => {
    const {state, props} = this;

    props.onUpdate(
      filterList(
        props.arrayToFilter,
        getFilterOptions(
          props.type,
          this.getFilteredValues(),
          props.filteredKey,
          props.type === 'number' ?
            selections[state.selectedfilterOptionKey] : null
        ))
      );
  }

  /*
    Updates state key with new value.
  */
  updateSelection = (value, key) => {
    this.setState({
      [key]: value
    })
  }

  toggleChecked = () => {
    this.setState({
      filterOn: !this.state.filterOn
    });
  }

  setValue = (param, e) => {
    this.setState({
      [param]: e.target.value
    });

    this.onUpdate();
  }

  getFilteredValues = () => {
    const {state, props} = this;

    if (props.type === 'number'){
      if (state.selectedfilterOptionKey[0]){ //between numbers
        return [state.lowerThan, state.higherThan];
      } else{
        return [state.singleNumericFilterValue];
      }
    }

    else if (props.type === 'text'){
      return [state.stringFilter];
    }

    //TODO: add boolean handling
  }
}

Filter.propTypes = {
  label: PropTypes.string,
  type: PropTypes.string.isRequired, //'text', 'number', 'bool'
  filteredKey: PropTypes.string.isRequired,
  arrayToFilter: PropTypes.array.isRequired,
  onUpdate: PropTypes.func.isRequired
};

export default Filter;
