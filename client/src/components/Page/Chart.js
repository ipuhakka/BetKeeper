import React, { Component } from 'react';
import PropTypes from 'prop-types';
import _ from 'lodash';
import { BarChart, Bar, PieChart, Pie, CartesianGrid, XAxis, YAxis, ResponsiveContainer, Tooltip } from 'recharts';
import Dropdown from './Dropdown';
import StaticTable from './StaticTable';
import './Chart.css';

/** Color palette to color data items */
const colors = [
    '#000000',
    '#fff100',
    '#ff8c00',
    '#e81123',
    '#ec008c',
    '#68217a',
    '#00188f',
    '#00bcf2',
    '#00b294',
    '#009e49'
];

class Chart extends Component
{
    constructor(props)
    {
        super(props);

        this.state = {
            activeDataField: props.dataFields[0],
            activeChart: 'bar',
            data: []
        };

        this.onDataFieldChange = this.onDataFieldChange.bind(this);
    }

    componentDidUpdate()
    {
        if (!_.isEqual(this.props.data, this.state.data))
        {
            this.setState({ data: this.props.data });
        }
    }

    onDataFieldChange(componentKey, newActiveKey)
    {
        const { dataFields } = this.props;
        this.setState({ activeDataField: dataFields.find(field => field.fieldKey === newActiveKey)});
    }

    renderChartSelection()
    {
        const { activeChart } = this.state;
        const options = [
            { key: 'bar', value: 'Bar chart', initialValue: activeChart === 'bar' },
            { key: 'pie', value: 'Pie chart', initialValue: activeChart === 'pie' },
            { key: 'table', value: 'Table', initialValue: activeChart === 'table' }
        ];

        // Handle chart selection change
        const onChange = (componentKey, newValue) => 
        {
            this.setState({ activeChart: newValue });
        }

        return <Dropdown 
        options={options}
        componentKey='chart-selection-dropdown'
        onChange={onChange}/>;
    }

    renderChart()
    {
        const { activeChart, activeDataField, data } = this.state;
        const { keyField, dataFields } = this.props;

        // Set color for item
        for (var i = 0; i < data.length; i++)
        {
            data[i].fill = colors[i % colors.length];
        }

        switch (activeChart)
        {
            case 'bar':
                return <BarChart barSize={20} barGap={2} data={data}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis interval={0} dataKey={keyField.fieldKey} />
                <YAxis/>
                <Tooltip />
                <Bar name={activeDataField.fieldLegend} dataKey={activeDataField.fieldKey} />
            </BarChart>;

            case 'pie':
                 return <PieChart width={730} height={250}>
                    <Pie
                        data={data} 
                        dataKey={activeDataField.fieldKey} 
                        nameKey={keyField.fieldKey} 
                        cx="50%" 
                        cy="50%" 
                        innerRadius={60} 
                        outerRadius={80}
                        label />
                    <Tooltip />
                  </PieChart>;

            case 'table':
                return <StaticTable 
                    data={data}
                    columns={dataFields} />;

            default:
                throw new Error(`${activeChart} not implemented`);
        }
    }

    render()
    {
        const { activeDataField } = this.state;
        const { dataFields } = this.props;

        const options = dataFields.map(field => {
            return { key: field.fieldKey, value: field.fieldLegend, initialValue: field.fieldKey === activeDataField.fieldKey };
        });
        
        return <div>
            {this.renderChartSelection()}
            <Dropdown 
                options={options}
                componentKey={`chart-field-selection-dropdown`}
                onChange={this.onDataFieldChange}/>
            <div className='page-chart'>
            <ResponsiveContainer>
                {this.renderChart()}
            </ResponsiveContainer>
        </div>
    </div>;
    }
};

Chart.propTypes = {
    data: PropTypes.array.isRequired,
    keyField: PropTypes.shape({
        fieldKey: PropTypes.string.isRequired,
        fieldType: PropTypes.string.isRequired,
        fieldLegend: PropTypes.string.isRequired
    }).isRequired,
    dataFields: PropTypes.arrayOf(
        PropTypes.shape({
            fieldKey: PropTypes.string.isRequired,
            fieldType: PropTypes.string.isRequired,
            fieldLegend: PropTypes.string.isRequired
        })),
};

export default Chart;