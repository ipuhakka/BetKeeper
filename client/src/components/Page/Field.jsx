import React, { Component } from 'react';
import PropTypes from 'prop-types';
import _ from 'lodash';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import Form from 'react-bootstrap/Form';
import Dropdown from './Dropdown';
import consts from '../../js/consts';
import * as utils from '../../js/utils';
import './Field.css';

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

        const newDateAsMoment = moment(newDate, consts.DATEPICKER_FORMAT);

        if (newDateAsMoment.isValid())
        {
            this.onChange(newDate);
        }
    }

    /**
     * On change event. Calls prop change handler with key and new value parameters.
     */
    onChange(newValue)
    {
        const { props } = this;

        const isValid = props.type === 'Integer'
        ? newValue.length <= 0  || utils.isInteger(newValue)
        : true;

        this.setState({
            value: newValue,
            isValid
        });

        if (isValid)
        {
            const dataValue = props.type === 'DateTime'
                ? moment(newValue).utc()
                : newValue;

            props.onChange(props.componentKey, dataValue);
        }
        else 
        {
            props.onError();
        }
        
    }

    renderDateTimeInput()
    {   
        const { minimumDateTime } = this.props;

        const minDate = _.isNil(minimumDateTime)
            ? null
            : new Date(minimumDateTime);

        return <DatePicker
            className='datetime-input'
            selected={this.state.value}
            showTimeSelect
            timeFormat="HH:mm"
            timeIntervals={30}
            minDate={minDate}
            isClearable
            onChange={(date) => this.setDateTime(date)}
            onChangeRaw={(input) => this.setDateTime(input)}
            dateFormat={consts.DATEPICKER_FORMAT}/>;
    }

    renderInput()
    {
        const  { props, state } = this;

        const type = _.includes(['Integer', 'Double'], props.type)
            ? 'number'
            : 'text'
            
        const as = props.type === 'TextArea'
            ? 'textarea'
            : 'input';

        return <Form.Control 
            type={type}
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
        const { type, label, onHandleDropdownServerUpdate } = this.props;

        let input;

        switch (type)
        {
            default: 
                throw new Error(`Non implemented input type ${type}`);

            case "Integer":
            case "Double":
            case 'TextArea':
            case 'TextBox':
                input = this.renderInput();
                break;

            case 'DateTime':
                input = this.renderDateTimeInput();
                break;

            case 'Dropdown':
                input = <Dropdown 
                    onChange={this.onChange} 
                    handleServerUpdate={onHandleDropdownServerUpdate}
                    {...this.props}/>;
        }

        return <div className='input-field-wrapper'>
            <label className='input-label'>{label}</label>
            <div className='input-wrapper'>{input}</div>
        </div>
    }
};

Field.propTypes = {
    type: PropTypes.oneOf([
        "Integer", 
        "Double", 
        "DateTime", 
        "TextBox", 
        "TextArea",
        "Dropdown"]).isRequired,
    label: PropTypes.string.isRequired,
    componentKey: PropTypes.string.isRequired,
    onChange: PropTypes.func.isRequired,
    onError: PropTypes.func,
    initialValue: PropTypes.string,
    readOnly: PropTypes.bool,
    onHandleDropdownServerUpdate: PropTypes.func
};

export default Field;