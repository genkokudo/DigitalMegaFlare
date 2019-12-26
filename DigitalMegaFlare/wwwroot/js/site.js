// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
class App extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            text: 'Hello Work!'
        }
    }

    render() {
        return (
            <div>
                <p> {this.state.text} </p>
            </div>
        )
    }
}

ReactDOM.render(
    <App />,
    document.getElementById('doodle')
);