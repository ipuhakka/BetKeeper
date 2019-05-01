import React, { Component } from 'react';
import PropTypes from 'prop-types';
import ControlLabel from 'react-bootstrap/lib/ControlLabel';
import FormControl from 'react-bootstrap/lib/FormControl';
import Checkbox from 'react-bootstrap/lib/Checkbox';
import Dropdown from '../Dropdown/Dropdown.jsx';
import './Filter.css';

const selections =
  [
    'Between',
    'Over',
    'Under'
  ];

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
            <Checkbox className="small" onChange={this.toggleChecked}/>
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
                <FormControl disabled={!state.filterOn} className="small" type="number" value={state.higherThan}
                  onChange={this.setValue.bind(this, "higherThan")}/>
                <FormControl disabled={!state.filterOn} className="small" type="number" value={state.lowerThan}
                  onChange={this.setValue.bind(this, "lowerThan")}/>
              </div>;
    }
    else {
      return <FormControl disabled={!state.filterOn} className="large" type="number" value={state.singleNumericFilterValue}
                onChange={this.setValue.bind(this, "singleNumericFilterValue")}/>;
    }

  }

  renderTextFilter = () => {
    const {props, state} = this;

    return(
        <div>
          <ControlLabel className="small">{props.label}</ControlLabel>
          <FormControl disabled={!state.filterOn} className="large" type="text" value={state.stringFilter}
           onChange={this.setValue.bind(this, "stringFilter")}/>
        </div>);
  }

  /*
  * Returns an array of filterOptions with key on which filter is applied,
  * value used in filter and filter option ('over', 'under', 'is', 'contains').
  */
  onUpdate = () => {
    {state, props} = this;

    //TODO: create and return filters to parent component
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

    //TODO: onupdate must be called here
  }
}

/*Filter.propTypes = {
  arrayToFilter: PropTypes.array.isRequired,
  filteredKey: PropTypes.string.isRequired
};*/

Filter.propTypes = {
  label: PropTypes.string,
  type: PropTypes.string.isRequired //'text', 'number', 'bool'
};

export default Filter;
