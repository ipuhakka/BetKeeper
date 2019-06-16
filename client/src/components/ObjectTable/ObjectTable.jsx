import React, { Component } from 'react';
import PropTypes from 'prop-types';
import Table from 'react-bootstrap/Table';

/*
  Returns a table where object keys are table rows.
*/
class ObjectTable extends Component{

  render(){
    const {tableTitle} = this.props;

    return(
      <Table size="sm" hover>
        <thead>
          <tr>
            <th>{tableTitle}</th>
          </tr>
        </thead>
        <tbody>{this.rows()}</tbody>
      </Table>
    );
  }

  rows = () => {

    const {data} = this.props;

    const keys = Object.keys(data).filter(key => this.findTitle(key) !== null);

    let i = -1;
    return keys.map(key => {
      i = i + 1;
      return (
        <tr key={i}>
          <td>{this.findTitle(key)}</td>
          <td>{data[key]}</td>
        </tr>);
    })

  }

  /* Returns a value from props.titles which matches key.
  Returns null when no title is found.*/
  findTitle = (key) => {

    const {titles} = this.props;

    const titleOption = titles.find(title => title.key === key);

    return titleOption === undefined ?
      null : titleOption.value;
  }
};

ObjectTable.propTypes = {

  data: PropTypes.object.isRequired,

  /* Label for data field. If no label is found,
    property is not displayed in table.*/
  titles: PropTypes.arrayOf(
    PropTypes.shape({
      key: PropTypes.string.isRequired,
      value: PropTypes.string.isRequired
    })).isRequired,

    tableTitle: PropTypes.string
}

export default ObjectTable;
