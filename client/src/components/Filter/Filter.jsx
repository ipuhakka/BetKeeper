import React, { Component } from 'react';
import PropTypes from 'prop-types';
import _ from 'lodash';
import Form from 'react-bootstrap/Form';
import DatePicker from 'react-datepicker';
import './Filter.css';
import {getFilterOptions} from '../../js/filter.js';
import consts from '../../js/consts.js';
import moment from 'moment';

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

      case 'dateTime':
        return this.renderDateTimeFilter();
      default:
        return null;
    }
  }

  renderDateTimeFilter = () =>
  {
      const {state, props} = this;

      return <div className='filter'>
        <Form.Label className='label'>{props.label}</Form.Label>

        <DatePicker
          className='datetime-input'
          selected={state.lowerLimit}
          startDate={null}
          showTimeSelect
          timeFormat="HH:mm"
          timeIntervals={15}
          isClearable
          onChange={(date) => this.setDateTime(date, 'lowerLimit')}
          onChangeRaw={(input) => this.setDateTime(input, 'lowerLimit')}
          dateFormat={consts.DATETIME_FORMAT}/>

        <DatePicker
          className='datetime-input'
          selected={state.upperLimit}
          startDate={null}
          showTimeSelect
          timeFormat="HH:mm"
          timeIntervals={15}
          isClearable
          onChange={(date) => this.setDateTime(date, 'upperLimit')}       
          onChangeRaw={(input) => this.setDateTime(input, 'upperLimit')}
          dateFormat={consts.DATETIME_FORMAT}/>
      </div>;
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
           value={state.lowerLimit} onChange={this.setValueFromEvent.bind(this, "lowerLimit")}/>
          <Form.Control disabled={!state.filterOn} type="number"
            value={state.upperLimit} onChange={this.setValueFromEvent.bind(this, "upperLimit")}/>
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
          onChange={this.setValueFromEvent.bind(this, "stringFilter")}/>
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

  /**
   * Sets a datetime value, if newDate is a valid datetime.
   * @param {object} newDate
   * @param {string} key
   */
  setDateTime = (newDate, key) => {

    if (!newDate)
    {
      this.setValue(key, null);
      
      return;
    }

    const newDateAsMoment = moment(newDate, consts.DATETIME_FORMAT);

    if (newDateAsMoment.isValid())
    {
      this.setValue(key, newDate);
    }
  }

  /*
  * Updates selected filter values for boolean selection.
  */
  changeBooleanSelection = (value, e) => {
    const { booleanFilters } = this.state;

    if (e.target.checked)
    {
      booleanFilters.push(value);

      this.setValue('booleanFilters', booleanFilters);
      return;
    }

    _.remove(booleanFilters, filter => _.isEqual(filter, value));

    this.setValue('booleanFilters', booleanFilters);
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

  /**
   * Sets a value of e.target to state
   */
  setValueFromEvent = (param, e) =>
  {
    this.setValue(param, e.target.value);
  }

  /**
   * Sets key to state with given value and 
   * calls onUpdate after state change.
   * @param {string} key
   * @param {any} value
   */
  setValue = (key, value) => 
  {
    this.setState({
      [key]: value
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

      case 'dateTime':
        return [state.lowerLimit, state.upperLimit];

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
