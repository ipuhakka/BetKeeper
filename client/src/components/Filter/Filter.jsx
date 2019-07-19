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
      booleanFilters: [],
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

    switch (props.type)
    {
      case 'number':
        return this.renderNumericFilter();

      case 'string':
        return this.renderTextFilter();

      case 'boolean':
        return this.renderBooleanFilter();

      default:
        return null;
    }
  }

  renderBooleanFilter = () =>
  {
    const {state, props} = this;

    if (_.isNil(props.valueList))
    {
      throw new Error('No valuelist prop provided');
    }

    const checks = _.map(props.valueList, row => {
      return <div className='boolean-check-div' key={`div_${row.value}`}>
          <Form.Label key={`label_${row.value}`} className='label'>{row.legend}</Form.Label>
          <Form.Check
            className='boolean-check'
            inline
            key={`check_${row.value}`}
            onChange={this.changeBooleanSelection.bind(this, row.value)}/>
        </div>;
    });

    return(
      <div className={state.filterOn ? 'filter checked' : 'filter unchecked'}>
        {checks}
    </div>);
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
  * Updates selected filter values for boolean selection.
  */
  changeBooleanSelection = (value, e) => {
    const { booleanFilters } = this.state;

    if (e.target.checked)
    {
      booleanFilters.push(value);

      this.setState({
        booleanFilters
      });


      this.onUpdate();
      return;
    }

    _.remove(booleanFilters, filter => _.isEqual(filter, value));

    this.setState({
      booleanFilters
    });


    this.onUpdate();
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

    switch (props.type)
    {
      case 'number':
        const lowerLimit = state.lowerLimit === ''
          ? null : state.lowerLimit;

        const upperLimit = state.upperLimit === ''
          ? null : state.upperLimit;

        return [lowerLimit, upperLimit];

      case 'string':
        return [state.stringFilter];

      case 'boolean':
        return state.booleanFilters;

      case 'datetime':
        throw new Error('datetime not implemented');

      default:
        return;
    }
  }
}

Filter.propTypes = {
  label: PropTypes.string.isRequired,
  type: PropTypes.oneOf(['string', 'number', 'boolean', 'dateTime']).isRequired,
  filteredKey: PropTypes.string.isRequired,
  arrayToFilter: PropTypes.array.isRequired,
  onUpdate: PropTypes.func.isRequired,
  valueList: PropTypes.arrayOf(PropTypes.shape({
    value: PropTypes.any,
    legend: PropTypes.string.isRequired
  }))
};

export default Filter;
