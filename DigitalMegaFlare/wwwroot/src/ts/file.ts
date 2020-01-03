// とりあえず、クラスを作ってみて他のtsでインポートできるか確認すること
// →できませんでした。やるにはバンドルすることが必要。

/**
 * 入力をそのまま返す
 * @param message メッセージ
*/
export function echo(message: string): string {
    return message;
}
