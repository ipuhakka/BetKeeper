import React, { Component } from 'react';
import PropTypes from 'prop-types';
import _ from 'lodash';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import Form from 'react-bootstrap/Form';
import Dropdown from './Dropdown';
import consts from '../../js/consts';
import * as utils from '../../js/utils';
import InputDropdown from './InputDropdown';
import MultiSelectDropdown from './MultiSelectDropdown';
import * as autoFormatter from '../../js/autoformatter';
import './Page.css';

class Field extends Component
{
    constructor(props)
    {
        super(props);

        this.onChange = this.onChange.bind(this);
        this.setDateTime = this.setDateTime.bind(this);
        this.state = {
            value: props.initialValue,
            isValid: true
        };
    }

    componentDidMount()
    {
        const { fieldType, initialValue } = this.props;

        if (fieldType === 'DateTime' 
            && (_.isNil(initialValue) || initialValue === ''))
        {
            this.setDateTime(new Date());
        }
    }

    /**
     * Check if initialValue has changed.
     */
    componentDidUpdate(prevProps)
    {
        if (!_.isEqual(prevProps.initialValue, this.props.initialValue))
        {
            this.setState({
                value: this.props.type === 'DateTime'
                    ? _.isNil(this.props.initialValue)
                        ? null
                        : moment(this.props.initialValue).local().toDate()
                    : this.props.initialValue
            });
        }
    }

    /**
     * Sets a datetime value, if newDate is a valid datetime.
     * @param {object} newDate
     */
    setDateTime(newDate)
    {
        if (!newDate)
        {
            this.onChange(null);
            return;
        }

        const asMoment = moment(newDate, consts.DATEPICKER_FORMAT);

        if (asMoment.isValid())
        {
            newDate = asMoment.utc().toDate();
            this.onChange(newDate);
        }
    }

    /**
     * On change event. Calls prop change handler with data path and new value parameters.
     */
    onChange(newValue)
    {
        const { props } = this;

        const isValid = props.type === 'Integer'
        ? newValue.length <= 0  || utils.isInteger(newValue)
        : true;

        if (props.autoFormatter)
        {
            newValue = autoFormatter.format(props.autoFormatter, newValue, this.state.value);
        }

        this.setState({
            value: newValue,
            isValid
        });

        if (isValid)
        {
            /** Join dataPath and component key */
            const dataPath = _.compact([props.dataPath, props.componentKey]).join('.');
            props.onChange(dataPath, newValue);
        }
    }

    renderDateTimeInput()
    {   
        const { minimumDateTime, readOnly } = this.props;

        const minDate = _.isNil(minimumDateTime)
            ? null
            : new Date(minimumDateTime);

        const dateValue = typeof this.state.value === 'string'
            ? new Date(this.state.value)
            : this.state.value;

        return <DatePicker
            readOnly={readOnly}
            className={`datetime-input${readOnly ? ' disabled': ''}`}
            selected={dateValue}
            showTimeSelect
            timeFormat='HH:mm'
            timeIntervals={30}
            minDate={minDate}
            isClearable={!readOnly}
            onChange={(date) => this.setDateTime(date)}
            onChangeRaw={(input) => this.setDateTime(input)}
            dateFormat={consts.DATEPICKER_FORMAT}/>;
    }

    renderInput()
    {
        const  { props, state } = this;

        const getType = () => 
        {
            if (_.includes(['Integer', 'Double'], props.type))
            {
                return 'number';
            }
            else if (props.hideText)
            {
                return 'password';
            }

            return 'text';
        }
            
        const as = props.type === 'TextArea'
            ? 'textarea'
            : 'input';

        return <Form.Control 
            type={getType()}
            disabled={props.readOnly}
            as={as}
            isInvalid={!state.isValid}
            value={state.value || ''}
            onChange={(e) => 
            {
                this.onChange(e.target.value);
            }} />
    }

    render()
    {
        const { type, label, onHandleDropdownServerUpdate, componentKey } = this.props;
        let input;

        let wrapperClassName = '';

        switch (type)
        {
            default: 
                throw new Error(`Non implemented input type ${type}`);

            case 'Integer':
            case 'Double':
            case 'TextArea':
            case 'TextBox':
                input = this.renderInput();
                break;

            case 'DateTime':
                input = this.renderDateTimeInput();
                break;

            case 'Dropdown':
                if (this.props.multipleSelection)
                {
                    input = <MultiSelectDropdown
                        {...this.props}  
                        onChange={this.onChange}
                        initialValues={this.props.initialValue}/>;
                }
                else
                {
                    // TODO: OnChange ei käytössä?
                    wrapperClassName='dropdown';
                    input = <Dropdown 
                        onChange={this.onChange} 
                        handleServerUpdate={onHandleDropdownServerUpdate}
                        {...this.props}/>;
                }
                break;

            case 'InputDropdown':
                input = <InputDropdown 
                    onChange={this.onChange} 
                    label={label}
                    componentKey={componentKey}
                    initialSelections={this.props.initialValue}/>;
                    break;

        }

        return <div className={`${wrapperClassName} input-field-wrapper`}>
            <label className='input-label'>{label}</label>
            <div className='input-wrapper'>{input}</div>
        </div>
    }
};

Field.propTypes = {
    type: PropTypes.oneOf([
        'Integer', 
        'Double', 
        'DateTime', 
        'TextBox', 
        'TextArea',
        'Dropdown',
        'InputDropdown']).isRequired,
    label: PropTypes.string.isRequired,
    componentKey: PropTypes.string.isRequired,
    onChange: PropTypes.func.isRequired,
    initialValue: PropTypes.any,
    readOnly: PropTypes.bool,
    onHandleDropdownServerUpdate: PropTypes.func,
    /** Parent container component keys joined with '.' Defines data path where components value is stored. */
    dataPath: PropTypes.string,
    /** Should the input be treated as a password input */
    hideText: PropTypes.bool,
    autoFormatter: PropTypes.oneOf(['Result'])
};

export default Field;