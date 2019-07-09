import React, { Component } from 'react';
import PropTypes from 'prop-types';
import _ from 'lodash';
import Form from 'react-bootstrap/Form';
import Dropdown from '../Dropdown/Dropdown.jsx';
import './Filter.css';
import {getFilterOptions} from '../../js/filter.js';

const numericSelections =
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

  componentDidUpdate(prevProps)
  {
    const {props} = this;

    if (!_.isEqual(props.arrayToFilter, prevProps.arrayToFilter))
    {
      this.onUpdate();
    }
  }

  render(){
    const {props} = this;

    if (props.type === "number")
    {
      return this.renderNumericFilter();
    }
    else if (props.type === "text")
    {
      return this.renderTextFilter();
    }
    else
    {
      return;
    }
  }

  renderNumericFilter = () =>
  {
    const {state, props} = this;

    const filters = state.selectedfilterOptionKey === 0
      ? <div className='input-field-small'>
          <Form.Control disabled={!state.filterOn} type="number"
           value={state.higherThan} onChange={this.setValue.bind(this, "higherThan")}/>
          <Form.Control disabled={!state.filterOn} type="number"
            value={state.lowerThan} onChange={this.setValue.bind(this, "lowerThan")}/>
        </div>
      : <Form.Control className='input-field' disabled={!state.filterOn} type="number" value={state.singleNumericFilterValue}
          onChange={this.setValue.bind(this, "singleNumericFilterValue")}/>;

    return(
        <div className={state.filterOn ? 'filter checked' : 'filter unchecked'}>
            <Form.Check className="check" onChange={this.toggleChecked}/>
            <Dropdown
              defaultKey={1} stateKey={"selectedfilterOptionKey"} id={1} className='dropdown'
                data={numericSelections} title={props.label} onUpdate={this.updateSelection}/>
            {filters}
        </div>);
  }

  renderTextFilter = () =>
  {
    const {props, state} = this;

    return(
      <div className={state.filterOn ? 'filter checked' : 'filter unchecked'}>
        <Form.Check className="check" onChange={this.toggleChecked}/>
        <p>{props.label}</p>
        <Form.Control disabled={!state.filterOn} type="text" value={state.stringFilter}
          onChange={this.setValue.bind(this, "stringFilter")}/>
    </div>);
  }

  /*
  * Returns new filterOptions.
  */
  onUpdate = (applyFilters) => {
    const {state, props} = this;

    props.onUpdate(
        getFilterOptions(
          props.type,
          this.getFilteredValues(),
          props.filteredKey,
          props.type === 'number' ?
            numericSelections[state.selectedfilterOptionKey] : null
        )
      );
  }

  /*
    Updates state key with new value.
  */
  updateSelection = (value, key) =>
  {
    this.setState({
      [key]: value
    }, () =>
    {
      if (this.state.filterOn)
      {
        this.onUpdate();
      }
    });
  }

  toggleChecked = () =>
  {
    const { props } = this;
    const filterOn = !this.state.filterOn;
    this.setState({ filterOn });

    if (filterOn)
    {
      this.onUpdate();
    }
    else
    {
      this.props.onUpdate(props.arrayToFilter);
    }
  }

  setValue = (param, e) =>
  {
    this.setState({
      [param]: e.target.value
    }, () => {
      this.onUpdate();
    });
  }

  getFilteredValues = () =>
  {
    const {state, props} = this;

    if (props.type === 'number')
    {
      if (state.selectedfilterOptionKey === 0)
      { //between numbers
        return [state.lowerThan, state.higherThan];
      }
      else
      {
        return [state.singleNumericFilterValue];
      }
    }

    else if (props.type === 'text')
    {
      return [state.stringFilter];
    }

    //TODO: add boolean handling
  }
}

Filter.propTypes =
{
  label: PropTypes.string.isRequired,
  type: PropTypes.string.isRequired, //'text', 'number', 'bool'
  filteredKey: PropTypes.string.isRequired,
  arrayToFilter: PropTypes.array.isRequired,
  onUpdate: PropTypes.func.isRequired
};

export default Filter;
