import React, { Component, Fragment } from 'react';
import PropTypes from 'prop-types';
import Accordion from 'react-bootstrap/Accordion';
import Button from 'react-bootstrap/Button';
import Card from 'react-bootstrap/Card';
import _ from 'lodash';
import Filter from '../../components/Filter/Filter.jsx';
import {filterList} from '../../js/filter.js';
import './Filters.css';

class Filters extends Component
{
  constructor(props)
  {
    super(props);

    this.state =
    {
      visibleArray: [],
      filterOptions: []
    }
  }

  componentDidMount()
  {
      this.setState({
        visibleArray: this.props.toFilter
      });
  }

  componentDidUpdate(prevProps)
  {
    const { props } = this;

    if (!_.isEqual(prevProps.toFilter, props.toFilter))
    {
      this.setState({
        visibleArray: props.toFilter
      })
    }
  }

  render()
  {
    return <Accordion>
      <Card.Header className='header'>
        <Accordion.Toggle className='toggleArrow' as={Button} eventKey="1">
          <i className="fas fa-chevron-down"></i>
        </Accordion.Toggle>
      </Card.Header>
      <Accordion.Collapse eventKey="1">
        <Card.Body className='body'>
          {this.renderFilters()}
        </Card.Body>
      </Accordion.Collapse>
    </Accordion>;
  }

  renderFilters = () =>
  {
    const { props } = this;

    const filters = [
      { key: 'bet', label: 'Bet', type:'number'},
      { key: 'odd', label: 'Odd', type: 'number'},
      { key: 'name', label: 'Name', type: 'string'}
    ];

   return <Fragment>
      {
        _.map(filters, filter => {
          return <Filter
            arrayToFilter={props.toFilter}
            onUpdate={this.onUpdate}
            type={filter.type}
            filteredKey={filter.key}
            label={filter.label}
            key={filter.key} />
        })
      }
      </Fragment>;
  }

  /*
  Handles a single filter update. Replaces
  previous filterOptions for key and Filters
  with new options.
  */
  onUpdate = (newFilterOption) =>
  {
    const { props, state } = this;
    let { filterOptions } = state;

    const newFilterKey = newFilterOption.key;

    _.remove(filterOptions, {
      key: newFilterKey
    });

    filterOptions.push(newFilterOption);

    const newFilteredArray = filterList(props.toFilter, filterOptions);

    if (!_.isEqual(newFilteredArray, state.visibleArray))
    {
      this.setState({
        visibleArray: newFilteredArray
      });

      this.props.onResultsUpdate(newFilteredArray);
    }
  }

};

Filters.propTypes =
{
    toFilter: PropTypes.array.isRequired,
    onResultsUpdate: PropTypes.func.isRequired
}

export default Filters;
