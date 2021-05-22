import React, { Component } from 'react';
import PropTypes from 'prop-types';
import Table from 'react-bootstrap/Table';

class StaticTable extends Component 
{
    renderTableRow(rowData, i)
    {
        const {componentKey} = this.props;
        const { cells } = rowData;

        const getStyles = (cell) => 
        {
            return {
                color: cell.color,
                fontWeight: cell.style === 'Bold'
                    ? 600
                    : 'normal'
            };
        }

        return <tr key={`tr-${componentKey}-${i}`}>{cells.map((cell, j) => 
            {
                return <td 
                    style={getStyles(cell)} 
                    key={`td-${i}-${j}`}>{cell.value}</td>;
            })}</tr>
    }

    render()
    {
        const { rows, header } = this.props;

        const tableRows = rows.map((row, i) => 
            {
                return this.renderTableRow(row, i);
            })

        const tableHeader = header
            ? <thead>
                <tr>
                    {header.cells.map(headerCell => {
                        return <th key={`header-cell-${headerCell.value}`}>{headerCell.value}</th>
                    })}
                </tr>
            </thead>
            : null;

        return <Table striped bordered size="sm">
            {tableHeader}
            <tbody>
                {tableRows}
            </tbody>
        </Table>;
    }
};

const cellProps = PropTypes.arrayOf(
    PropTypes.shape({
        color: PropTypes.string,
        style: PropTypes.string,
        value: PropTypes.string
    }));

StaticTable.propTypes = {
    componentKey: PropTypes.string,
    header: PropTypes.shape({
            cellProps
        }
    ),
    rows: PropTypes.arrayOf(
        PropTypes.shape({ cells: cellProps })
        )
}

export default StaticTable;