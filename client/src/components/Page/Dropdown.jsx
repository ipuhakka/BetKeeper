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

        this.onChange = this.onChange.bind(this);
    }

    componentDidMount()
    {
        const { options } = this.props;

        if (!options || options.length === 0)
        {
            return;
        }

        // Check if an option is declared as initialvalue
        const initialOption = _.find(options, option => option.initialValue);

        const initialOptionKey = !_.isNil(initialOption)
            ? initialOption.key
            : options[0].key;

        // Ignore server update on mounting
        this.onChange(initialOptionKey, true);
    }

    componentDidUpdate(prevProps)
    {
        const { options } = this.props;
        const { activeKey } = this.state;

        const prevPropsInitialOption = _.find(prevProps.options, option => option.initialValue);
        const newPropsInitialOption = _.find(options, option => option.initialValue);

        if (!_.isEqual(prevPropsInitialOption, newPropsInitialOption) && newPropsInitialOption)
        {
            this.onChange(newPropsInitialOption.key, true);
            return;
        }

        if (options.length > 0 
            && _.isNil(_.find(options, option => option.key === activeKey)))
        {
            this.onChange(options[0].key, true);
        }
    }

    /**
     * Handles value change
     * @param {*} newValue 
     * @param {boolean} ignoreServerUpdate 
     */
    onChange(newValue, ignoreServerUpdate)
    {
        const { props } = this;

        this.setState({
            activeKey: newValue
        });

        const dataPath = _.compact([props.dataPath, props.componentKey]).join('.');

        if (!ignoreServerUpdate && _.get(props, 'componentsToUpdate.length', 0) > 0)
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
                    onClick={() => 
                        {
                            this.onChange(option.key);
                        }} 
                    >{option.value}</DropdownItem>;
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
            value: PropTypes.string.isRequired,
            initialValue: PropTypes.bool.isRequired
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