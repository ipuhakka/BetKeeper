import React, { Component } from 'react';
import PropTypes from 'prop-types';
import _ from 'lodash';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import Form from 'react-bootstrap/Form';
import consts from '../../js/consts.js';
import './Input.css';

class Input extends Component
{
    constructor(props)
    {
        super(props);

        this.onChange = this.onChange.bind(this);
        this.setDateTime = this.setDateTime.bind(this);
        
        this.state = {
            value: null
        };
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
        this.setState({
            value: newValue
        });

        this.props.onChange(this.props.fieldKey, newValue);
    }

    renderDateTimeInput()
    {
        return <DatePicker
            className='datetime-input'
            selected={this.state.value}
            showTimeSelect
            timeFormat="HH:mm"
            timeIntervals={30}
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
            as={as}
            value={state.value || ''}
            onChange={(e) => 
            {
                this.onChange(e.target.value);
            }} />
    }

    render()
    {
        const { type, label } = this.props;

        let input;

        // TODO: Numeeriset tyypit ja validaattorit
        switch (type)
        {
            default: 
                throw new Error(`Non implemented input type ${type}`);

            case 'DateTime':
                input = this.renderDateTimeInput();
                break;

            case 'TextArea':
                input = this.renderInput();
                break;

            case 'TextBox':
                input = this.renderInput();
                break;
        }

        return <div className='input-field-wrapper'>
            <label className='input-label'>{label}</label>
            <div className='input-wrapper'>{input}</div>
        </div>
    }
};

Input.propTypes = {
    type: PropTypes.oneOf([
        "Integer", 
        "Double", 
        "DateTime", 
        "TextBox", 
        "TextArea"]).isRequired,
    label: PropTypes.string.isRequired,
    fieldKey: PropTypes.string.isRequired,
    onChange: PropTypes.func.isRequired
};

export default Input;