import React, { Component } from 'react';
import Select from 'react-select';
import PropTypes from 'prop-types';
import _ from 'lodash';

class MultiSelectDropdown extends Component
{
    constructor(props)
    {
        super(props);

        const { options } = this.props;

        if (_.isNil(options))
        {
            return;
        }

        const defaultValues = options
            .filter(option => option.initialValue)
            .map(option => { 
                return { 
                    value: option.key,
                    label: option.value
                }; 
            });

        this.state = {
            // Values selected by default
            defaultValues: defaultValues
        };

        this.onChange = this.onChange.bind(this);
    }

    /**
     * Event handler for changing dropdown selected items status
     * @param {*} selectedOptions 
     */
    onChange(selectedOptions)
    {
        const newValues = selectedOptions.map(option => option.value);
        this.props.onChange(newValues);
    }

    render()
    {
        const { options, label } = this.props;
        const { defaultValues } = this.state;

        const optionProps = options.map(option => 
        {
            return { value: option.key, label: option.value };
        });

        return <Select
            defaultValue={defaultValues}
            options={optionProps}
            isMulti={true}
            placeholder={label}
            onChange={this.onChange}/>;
    }
};

MultiSelectDropdown.propTypes = {
    options: PropTypes.arrayOf(
        PropTypes.shape({
            key: PropTypes.string.isRequired,
            value: PropTypes.string.isRequired,
            initialValue: PropTypes.bool.isRequired
        })
    ),
    onChange: PropTypes.func,
    componentKey: PropTypes.string.isRequired
};

export default MultiSelectDropdown;