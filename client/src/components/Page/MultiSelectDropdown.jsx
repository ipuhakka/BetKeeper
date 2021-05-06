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

        let defaultValues = options
            .filter(option => option.initialValue)
            .map(option => { 
                return { 
                    value: option.key,
                    label: option.value
                }; 
            });

        // If default values are not specified in component model, they can be in data
        if (defaultValues.length === 0 && Array.isArray(props.initialValue) && props.initialValue.length > 0)
        {
            defaultValues = options
                .filter(option => props.initialValue.includes(option.key))
                .map(option => { 
                    return { 
                        value: option.key,
                        label: option.value
                    }; 
                });   
        }

        this.state = {
            // Values selected by default
            defaultValues: defaultValues,
            selectedCount: defaultValues.length
        };

        this.onChange = this.onChange.bind(this);
        this.areSelectionsDisabled = this.areSelectionsDisabled.bind(this);
    }

    /**
     * Event handler for changing dropdown selected items status
     * @param {*} selectedOptions 
     */
    onChange(selectedOptions)
    {
        const newValues = selectedOptions 
            ? selectedOptions.map(option => option.value)
            : null;

        const { onChange } = this.props;

        this.setState({
            selectedCount: selectedOptions.length
        });

        onChange(newValues);
    }

    /** Function for checking is making a new selection is allowed */
    areSelectionsDisabled()
    {
        const { selectedCount } = this.state;
        const { allowedSelectionCount } = this.props;

        return selectedCount === allowedSelectionCount;
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
            isOptionDisabled={this.areSelectionsDisabled}
            defaultValue={defaultValues}
            options={optionProps}
            isMulti={true}
            placeholder={label}
            onChange={this.onChange}
            closeMenuOnSelect={false} />;
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
    componentKey: PropTypes.string.isRequired,
    allowedSelectionCount: PropTypes.number
};

export default MultiSelectDropdown;