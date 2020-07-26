import React, { Component } from 'react';
import PropTypes from 'prop-types';
import _ from 'lodash';
import Button from 'react-bootstrap/Button';
import DropdownButton from 'react-bootstrap/DropdownButton';
import DropdownItem from 'react-bootstrap/DropdownItem';
import Form from 'react-bootstrap/Form';
import './Field.css';

class InputDropdown extends Component 
{
    constructor(props){
        super(props);

        this.state = {
            newValue: '',
            values: []
        }

        this.addSelection = this.addSelection.bind(this);
        this.removeSelection = this.removeSelection.bind(this);

        this.inputRef = React.createRef();
    }

    /**
     * Set initial selections if they exist.
     */
    componentDidMount()
    {
        const { initialSelections } = this.props;

        if (_.isNil(initialSelections))
        {
            return;
        }
        
        this.setState({
            values: initialSelections
        })
    }

    /**
     * Check if initialSelections have changed.
     */
    componentDidUpdate(prevProps)
    {
        if (!_.isEqual(prevProps.initialSelections, this.props.initialSelections))
        {
            this.setState({
                values: this.props.initialSelections
            });
        }
    }

    /**
     * Adds a new selection.
     */
    addSelection()
    {
        const { newValue } = this.state;
        let values = _.clone(this.state.values);

        values.push(newValue);

        values = values.sort();

        this.setState({
            values,
            newValue: ''
        });

        this.props.onChange(values);
    }

    /**
     * Remove value from selection
     * @param {string} valueToRemove 
     */
    removeSelection(valueToRemove)
    {
        const { values } = this.state;

        const filteredValues = values.filter(value => value !== valueToRemove); 

        this.setState({
            values: filteredValues
        });

        this.props.onChange(filteredValues);
    }

    /**
     * Render dropdown content.
     */
    renderItems()
    {
        const { values } = this.state;

        const items = [
            this.renderAddNewOptionsDiv()
        ];

        values.forEach((value, i) => 
        {
            items.push(<DropdownItem key={`dropdown-item-${i}`} className='add-selection-item'>
                <div className='add-selection-item-inner-div'>
                    <p className='add-selection-item-text'>{value}</p>
                    <i 
                        className="fas fa-times add-selection-item-remove"
                        onClick={(e) => 
                        {
                            this.removeSelection(value);
                            e.preventDefault();
                            e.stopPropagation();
                        }}>
                    </i>
                </div>
            </DropdownItem>);
        });
            
        return items;
    }

    /**
     * Renders field to add new options to dropdown
     */
    renderAddNewOptionsDiv()
    {
        return <DropdownItem key='dropdown-add-options' className='add-options-item'>
            <Form.Control
                ref={this.inputRef}
                value={this.state.newValue || ''}
                onChange={(e) => 
                {
                    this.setState({ newValue: e.target.value });
                }} 
                onClick={(e) => 
                {
                    // Prevent dropdown closing
                    e.preventDefault();
                    e.stopPropagation();
                }}
                onKeyDown={(e) => 
                    {
                        // Prevent dropdown closing on spacebar click
                        e.stopPropagation();
                    }}
            />
            <Button 
                onClick={(e) => 
                    {
                        this.addSelection();
                        
                        // Prevent dropdown closing
                        e.preventDefault();
                        e.stopPropagation();

                        // Focus on input element to allow writing another option instantly
                        this.inputRef.current.focus();
                    }}
                disabled={this.state.newValue === ''}>Add</Button>
        </DropdownItem>;
    }

    render()
    {
        const { props } = this;
        return (<DropdownButton 
            variant="outline-primary"
            title={props.label}>
                {this.renderItems()}
            </DropdownButton>);
    }
};

InputDropdown.propTypes = {
    componentKey: PropTypes.string.isRequired,
    label: PropTypes.string.isRequired,
    onChange: PropTypes.func.isRequired,
    initialSelections: PropTypes.arrayOf(PropTypes.string)
};

export default InputDropdown;