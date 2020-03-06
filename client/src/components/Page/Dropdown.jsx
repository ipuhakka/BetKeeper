import React, { Component } from 'react';
import PropTypes from 'prop-types';
import _ from 'lodash';
import DropdownButton from 'react-bootstrap/DropdownButton';
import DropdownItem from 'react-bootstrap/DropdownItem';

class Dropdown extends Component 
{
    constructor(props)
    {
        super(props);

        this.state = {
            activeKey: props.options[0].key
        }
    }

    onChange(newValue)
    {
        const { props } = this;

        this.setState({
            activeKey: newValue
        });

        props.onChange(props.fieldKey, newValue);
    }

    render()
    {
        const { options } = this.props;
        
        const { activeKey } = this.state;

        const items = _.map(options, option => 
            {
                return <DropdownItem 
                    active={option.key === activeKey} 
                    key={option.key}
                    onClick={this.onChange.bind(this, option.key)} >{option.value}</DropdownItem>;
            });

        return (<DropdownButton 
            variant="outline-primary"
            title={options[activeKey].value} >
                {items}
            </DropdownButton>);
    }
};

Dropdown.propTypes = {
    options: PropTypes.arrayOf(
        PropTypes.shape({
            key: PropTypes.string.isRequired,
            value: PropTypes.string.isRequired
        })
    ),
    onChange: PropTypes.func,
    fieldKey: PropTypes.string.isRequired
};

export default Dropdown;