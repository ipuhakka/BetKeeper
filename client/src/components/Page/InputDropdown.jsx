import React, { Component } from 'react';
import PropTypes from 'prop-types';
import _ from 'lodash';
import Button from 'react-bootstrap/Button';
import DropdownButton from 'react-bootstrap/DropdownButton';
import DropdownItem from 'react-bootstrap/DropdownItem';
import Form from 'react-bootstrap/Form';
import Modal from 'react-bootstrap/Modal';
import * as PageActions from '../../actions/pageActions';
import * as PageUtils from '../../js/pageUtils';
import './Page.css';

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
        this.toggleCopySelectionsModalState = this.toggleCopySelectionsModalState.bind(this);
        this.copySelectionsFromExistingList = this.copySelectionsFromExistingList.bind(this);

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
        });

        PageActions.updatePageDropdownSelections(
            PageUtils.getActivePageName(),
            this.props.componentKey,
            this.props.initialSelections);
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
     * Toggle copy selections modal state
     */
    toggleCopySelectionsModalState()
    {
        const { copySelectionsModalOpen } = this.state;

        this.setState({ copySelectionsModalOpen: !copySelectionsModalOpen });
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
        PageActions.updatePageDropdownSelections(PageUtils.getActivePageName(), this.props.componentKey, values);
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
        PageActions.updatePageDropdownSelections(PageUtils.getActivePageName(), this.props.componentKey, filteredValues);
    }

    /** Copy selections from existing list */
    copySelectionsFromExistingList(options)
    {
        this.setState({
            values: options,
            newValue: ''
        });

        this.props.onChange(options);
        PageActions.updatePageDropdownSelections(PageUtils.getActivePageName(), this.props.componentKey, options);
        this.toggleCopySelectionsModalState();
    }

    /**
     * Render copy selections modal
     */
    renderCopySelectionsModal()
    {
        const { existingSelections } = this.props;
        if (!existingSelections)
        {
            return;
        }

        const handledLists = [];

        return <Modal 
            show={this.state.copySelectionsModalOpen}
            onHide={this.toggleCopySelectionsModalState}
            className='input-dropdown-copy-selections-modal'>
            <Modal.Header closeButton>
            <Modal.Title>Copy options from existing list</Modal.Title>
            </Modal.Header>
        
            <Modal.Body>
                {Object.entries(existingSelections).map(([key, selections], i) =>
                {
                    if (selections.length === 0)
                    {
                        return null;
                    }

                    // Don't add option if same list already added
                    if (_.some(handledLists, handledList => 
                        {
                            return _.isEqual(handledList, selections);
                        }))
                    {
                        return null;
                    }

                    handledLists.push(selections);

                    return <div className='copy-selections-div' key={`${key}-selection-${i}`}>
                        <p>{selections.join(', ')}</p>
                        <Button onClick={() => 
                            {
                                this.copySelectionsFromExistingList(selections);
                            }} className='copy-selections-button'>Copy</Button>
                    </div>;
                })}
            </Modal.Body>
        
            <Modal.Footer>
            <Button variant="secondary" onClick={this.toggleCopySelectionsModalState}>Cancel</Button>
            </Modal.Footer>
        </Modal>;
    }

    /**
     * Render dropdown content.
     */
    renderItems()
    {
        const { values } = this.state;

        const items = [
            this.renderAddNewOptionsDiv(),
            (_.isNil(values) || values.length === 0) && <Button 
                key={'copy-button'}
                className='open-copy-selections-button'
                onClick={this.toggleCopySelectionsModalState}>Copy selections</Button>,
            <div key='dropdown-selection-div' className='dropdown-selection-div'>
            {        
                values.map((value, i) => 
                {
                    return <DropdownItem key={`dropdown-item-${i}`} className='add-selection-item'>
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
                </DropdownItem>;
                })
            }
        </div>
        ];

        return items;
    }

    /**
     * Renders field to add new options to dropdown
     */
    renderAddNewOptionsDiv()
    {
        return <DropdownItem key='dropdown-add-options' className='add-options-item'>
            <Form.Control
                className='add-options-input'
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

        return <div>
                {this.renderCopySelectionsModal()}
                <DropdownButton
                    variant="outline-primary"
                    title={props.label}>
                {this.renderItems()}
            </DropdownButton>
        </div>;
    }
};

InputDropdown.propTypes = {
    componentKey: PropTypes.string.isRequired,
    label: PropTypes.string.isRequired,
    onChange: PropTypes.func.isRequired,
    initialSelections: PropTypes.arrayOf(PropTypes.string),
    /** Existing input dropdown selections */
    existingSelections: PropTypes.object
};

export default InputDropdown;