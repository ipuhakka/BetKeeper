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
      valueListFilters: [],
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

      case 'valueList':
        return this.renderValueListFilter();

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

        <div>
          <DatePicker
            className='datetime-input'
            selected={state.lowerLimit}
            showTimeSelect
            timeFormat="HH:mm"
            timeIntervals={15}
            isClearable
            startDate={state.lowerLimit}
            endDate={state.upperLimit}
            onChange={(date) => this.setDateTime(date, 'lowerLimit')}
            onChangeRaw={(input) => this.setDateTime(input, 'lowerLimit')}
            dateFormat={consts.DATEPICKER_FORMAT}/>

          <DatePicker
            className='datetime-input'
            selected={state.upperLimit}
            showTimeSelect
            timeFormat="HH:mm"
            timeIntervals={15}
            isClearable            
            startDate={state.lowerLimit}
            endDate={state.upperLimit}
            onChange={(date) => this.setDateTime(date, 'upperLimit')}       
            onChangeRaw={(input) => this.setDateTime(input, 'upperLimit')}
            dateFormat={consts.DATEPICKER_FORMAT}/>
          </div>
      </div>;
  }

  renderValueListFilter = () =>
  {
    const {state, props} = this;

    if (_.isNil(props.valueList))
    {
      throw new Error('No valuelist prop provided');
    }

    const checks = _.map(props.valueList, row => {
      return <div className='valueList-check-div' key={`div_${row.value}`}>
          <Form.Label key={`label_${row.value}`} className='label'>{row.legend}</Form.Label>
          <Form.Check
            className='valueList-check'
            inline
            key={`check_${row.value}`}
            onChange={this.changeValueListSelection.bind(this, row.value)}/>
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
  onUpdate = () =>
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

    const newDateAsMoment = moment(newDate, consts.DATEPICKER_FORMAT);

    if (newDateAsMoment.isValid())
    {
      this.setValue(key, newDate);
    }
  }

  /*
  * Updates selected filter values for valueList selection.
  */
  changeValueListSelection = (value, e) => {
    const { valueListFilters } = this.state;

    if (e.target.checked)
    {
      valueListFilters.push(value);

      this.setValue('valueListFilters', valueListFilters);
      return;
    }

    _.remove(valueListFilters, filter => _.isEqual(filter, value));

    this.setValue('valueListFilters', valueListFilters);
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

  /**
   * Clears filters and handles update.
   */
  clearFilters = () => 
  {
    this.setState({
      upperLimit: '',
      lowerLimit: '',
      stringFilter: ''
    }, () => {
      this.onUpdate();
    });
  }

  /**
   * Toggles a filter check.
   */
  toggleChecked = () =>
  {
    const filterOn = !this.state.filterOn;
    this.setState({ filterOn });

    if (filterOn)
    {
      this.onUpdate();
    }
    else
    {
      this.clearFilters();
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

      case 'valueList':
        return state.valueListFilters;

      case 'dateTime':
        return [state.lowerLimit, state.upperLimit];

      default:
        return;
    }
  }
}

Filter.propTypes = {
  label: PropTypes.string.isRequired,
  type: PropTypes.oneOf(['string', 'number', 'valueList', 'dateTime']).isRequired,
  filteredKey: PropTypes.string.isRequired,
  arrayToFilter: PropTypes.array.isRequired,
  onUpdate: PropTypes.func.isRequired,
  valueList: PropTypes.arrayOf(PropTypes.shape({
    value: PropTypes.any,
    legend: PropTypes.string.isRequired
  }))
};

export default Filter;
