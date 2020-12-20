import React, { Component } from 'react';
import PropTypes from 'prop-types';
import _ from 'lodash';
import ListGroupRB from 'react-bootstrap/ListGroup';
import ListGroupItem from 'react-bootstrap/ListGroupItem';
import './ListGroup.css';

class ListGroup extends Component
{
    constructor(props)
    {
        super(props);

        this.state = {
            selected: []
        }

        this.handleGroupItemClick = this.handleGroupItemClick.bind(this);
    }

    /**
     * Handle group item click
     * @param {*} itemIdentifier 
     */
    handleGroupItemClick(itemIdentifier)
    {
        const { selected } = this.state;
        const { mode, onSelect, dataPath, componentKey } = this.props;

        if (mode === 'DisplayOnly')
        {
            return;
        }

        if (mode === 'Selectable')
        {
            const newSelected = _.xor(selected, [itemIdentifier]);
            this.setState(
            {
                selected: newSelected
            });

            /** Join dataPath and component key */
            const dataKey = _.compact([dataPath, componentKey]).join('.');
            onSelect(dataKey, newSelected);
        }
    }

    render()
    {
        const { data, headerKeys, smallItemKeys, keyField, mode } = this.props;

        const items = data.map((dataItem, i) =>
            {
                return <ListGroupItem 
                    action={mode !== 'DisplayMode'}
                    variant={this.state.selected.includes(dataItem[keyField]) ? 'info': null}
                    onClick={() => 
                    {
                        this.handleGroupItemClick(dataItem[keyField]); 
                    }}
                    key={`${keyField}-${i}`}>
                    <div>{headerKeys.map(key => dataItem[key]).join(' ')}</div>
                    <div className='small-div'>{smallItemKeys || [].map(key => dataItem[key]).join(' ')}</div>
                </ListGroupItem>;
            });

        return <ListGroupRB>{items}</ListGroupRB>;
    }
};

ListGroup.propTypes = {
    data: PropTypes.arrayOf(PropTypes.object).isRequired,
    keyField: PropTypes.string.isRequired,
    headerKeys: PropTypes.arrayOf(PropTypes.string).isRequired,
    smallItemKeys: PropTypes.arrayOf(PropTypes.string),
    mode: PropTypes.string.isRequired,
    componentKey: PropTypes.string,
    dataPath: PropTypes.string,
    onSelect: PropTypes.func.isRequired
}

export default ListGroup;