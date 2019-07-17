import React, { Component } from 'react';
import PropTypes from 'prop-types';
import _ from 'lodash';
import Form from 'react-bootstrap/Form';
import './Filter.css';
import {getFilterOptions} from '../../js/filter.js';

class Filter extends Component{
  constructor(props){
    super(props);

    this.state = {
      upperLimit: '',
      lowerLimit: '',
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
    else if (props.type === "string")
    {
      return this.renderTextFilter();
    }

    return null;
  }

  renderNumericFilter = () =>
  {
    const {state, props} = this;

    const filters = <div className='inputs'>
          <Form.Control disabled={!state.filterOn} type="number"
           value={state.lowerLimit} onChange={this.setValue.bind(this, "lowerLimit")}/>
          <Form.Control disabled={!state.filterOn} type="number"
            value={state.upperLimit} onChange={this.setValue.bind(this, "upperLimit")}/>
        </div>;

    return(
        <div className={state.filterOn ? 'filter checked' : 'filter unchecked'}>
            <Form.Check className="check" onChange={this.toggleChecked}/>
            <Form.Label className='label'>{props.label}</Form.Label>
            {filters}
        </div>);
  }

  renderTextFilter = () =>
  {
    const {props, state} = this;

    return(
      <div className={state.filterOn ? 'filter checked' : 'filter unchecked'}>
        <Form.Check className="check" onChange={this.toggleChecked}/>
        <Form.Label className='label'>{props.label}</Form.Label>
        <Form.Control className='inputs' disabled={!state.filterOn} type="text" value={state.stringFilter}
          onChange={this.setValue.bind(this, "stringFilter")}/>
    </div>);
  }

  /*
  * Returns new filterOptions.
  */
  onUpdate = (applyFilters) =>
  {
    const {props} = this;

    props.onUpdate(
        getFilterOptions(
          props.type,
          props.filteredKey,
          this.getFilteredValues()
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
      const lowerLimit = state.lowerLimit === ''
        ? null : state.lowerLimit;

      const upperLimit = state.upperLimit === ''
        ? null : state.upperLimit;

      return [lowerLimit, upperLimit];
    }

    else if (props.type === 'string')
    {
      return [state.stringFilter];
    }

    //TODO: add boolean handling

    //TODO: add dateTime handling
  }
}

Filter.propTypes =
{
  label: PropTypes.string.isRequired,
  type: PropTypes.oneOf(['string', 'number', 'boolean', 'dateTime']).isRequired,
  filteredKey: PropTypes.string.isRequired,
  arrayToFilter: PropTypes.array.isRequired,
  onUpdate: PropTypes.func.isRequired
};

export default Filter;
