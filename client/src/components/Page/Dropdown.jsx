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
            activeKey: _.get(props, 'options[0].key', null)
        }
    }

    componentDidMount()
    {
        const { props } = this;

        if (props.options.length === 0)
        {
            return;
        }

        const initialOption = _.find(props.options, option =>
            option.key === props.initialValue);

        const initialValue = _.isNil(initialOption)
            ? props.options[0].key
            : initialOption.key;

        this.onChange(initialValue);
    }

    componentDidUpdate()
    {
        const { options } = this.props;
        const { activeKey } = this.state;

        if (options.length > 0 
            && _.isNil(_.find(options, option => option.key === activeKey)))
        {
            this.onChange(options[0].key);
        }
    }

    onChange(newValue)
    {
        const { props } = this;

        this.setState({
            activeKey: newValue
        });

        const dataPath = _.compact([props.dataPath, props.componentKey]).join('.');

        if (_.get(props, 'componentsToUpdate.length', 0) > 0)
        {
            /** Join dataPath and component key */
            props.handleServerUpdate(props.componentKey, newValue, props.componentsToUpdate);
        }

        props.onChange(dataPath, newValue);
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

        const activeOption = _.find(options, option => option.key === activeKey);

        return (<DropdownButton 
            variant="outline-primary"
            title={_.isNil(activeOption)
                ? ''
                : activeOption.value} >
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
    componentKey: PropTypes.string.isRequired,
    componentsToUpdate: PropTypes.arrayOf(PropTypes.string),
    handleServerUpdate: PropTypes.func,
    /** Path consisting of parent Container splitted by '.'. 
     * Used to set data to correct path.  */
    dataPath: PropTypes.string
};

export default Dropdown;